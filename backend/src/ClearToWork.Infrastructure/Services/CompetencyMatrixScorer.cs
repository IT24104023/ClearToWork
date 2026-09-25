using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;
using Microsoft.Extensions.Options;

namespace ClearToWork.Infrastructure.Services;

/// <summary>
/// Configurable weights and parameters for the workforce competency scoring matrix.
/// Weights are normalized so they proportionally influence the final 0-100 score.
/// </summary>
public class CompetencyScoringWeights
{
    /// <summary>Weight assigned to valid certifications (default: 0.35 = 35%).</summary>
    public double CertificationsWeight { get; set; } = 0.35;

    /// <summary>Weight assigned to years of industry experience (default: 0.25 = 25%).</summary>
    public double ExperienceWeight { get; set; } = 0.25;

    /// <summary>Weight assigned to historical safety record / incident avoidance (default: 0.25 = 25%).</summary>
    public double SafetyRecordWeight { get; set; } = 0.25;

    /// <summary>Weight assigned to mandatory training completion percentage (default: 0.15 = 15%).</summary>
    public double TrainingWeight { get; set; } = 0.15;

    /// <summary>Number of years of experience required to achieve 100% in the experience category.</summary>
    public int MaxExperienceYearsCap { get; set; } = 10;

    /// <summary>Benchmark number of valid certifications required for full cert score.</summary>
    public int TargetCertsCount { get; set; } = 3;

    /// <summary>Penalty deduction per recorded safety incident.</summary>
    public double IncidentPenaltyPoints { get; set; } = 25.0;

    /// <summary>Minimum overall score threshold required to lead a high-risk permit.</summary>
    public double MinimumPermitApprovalScore { get; set; } = 60.0;
}

/// <summary>
/// Input metrics required to evaluate a worker's competency score.
/// </summary>
public record WorkerCompetencyInput(
    Guid? WorkerId,
    string WorkerName,
    int ValidCertificatesCount,
    int YearsOfExperience,
    int SafetyIncidentsCount,
    double TrainingCompletionRatePercent
);

/// <summary>
/// Breakdown of the calculated competency score across all matrix components.
/// </summary>
public record CompetencyScore(
    double OverallScore,
    string CompetencyLevel,
    bool IsPermitEligible,
    double CertificationsScore,
    double ExperienceScore,
    double SafetyRecordScore,
    double TrainingScore,
    double WeightedCertificationsScore,
    double WeightedExperienceScore,
    double WeightedSafetyScore,
    double WeightedTrainingScore,
    List<string> BreakdownNotes
);

/// <summary>
/// Contract for evaluating worker competency against the multi-factor safety matrix.
/// </summary>
public interface ICompetencyMatrixScorer
{
    /// <summary>
    /// Calculates competency score and full breakdown based on provided metrics.
    /// </summary>
    CompetencyScore ScoreWorker(WorkerCompetencyInput input);

    /// <summary>
    /// Overload that evaluates a <see cref="Worker"/> domain entity and supplied operational metrics.
    /// </summary>
    CompetencyScore ScoreWorker(
        Worker worker,
        int yearsOfExperience,
        int safetyIncidentCount,
        double trainingCompletionRate);
}

/// <summary>
/// Service implementing multi-factor workforce competency evaluation for Permit-to-Work clearance.
/// Synthesizes certifications, experience, safety incident history, and training compliance.
/// </summary>
public class CompetencyMatrixScorer : ICompetencyMatrixScorer
{
    private readonly CompetencyScoringWeights _weights;

    public CompetencyMatrixScorer(IOptions<CompetencyScoringWeights>? options = null)
    {
        _weights = options?.Value ?? new CompetencyScoringWeights();
    }

    public CompetencyMatrixScorer(CompetencyScoringWeights weights)
    {
        _weights = weights ?? throw new ArgumentNullException(nameof(weights));
    }

    /// <inheritdoc />
    public CompetencyScore ScoreWorker(WorkerCompetencyInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var notes = new List<string>();

        // 1. Valid Certifications Component (0 - 100)
        var certsScore = CalculateCertificationsScore(input.ValidCertificatesCount);
        notes.Add($"Certifications: {input.ValidCertificatesCount} active ({certsScore:F1} / 100). Target: {_weights.TargetCertsCount}+.");

        // 2. Years of Experience Component (0 - 100)
        var expScore = CalculateExperienceScore(input.YearsOfExperience);
        notes.Add($"Experience: {input.YearsOfExperience} yrs ({expScore:F1} / 100). Benchmark: {_weights.MaxExperienceYearsCap} yrs.");

        // 3. Safety Incidents History Component (0 - 100)
        var safetyScore = CalculateSafetyScore(input.SafetyIncidentsCount);
        if (input.SafetyIncidentsCount > 0)
        {
            notes.Add($"Safety Record: {input.SafetyIncidentsCount} past incident(s) (-{input.SafetyIncidentsCount * _weights.IncidentPenaltyPoints} pts penalty) -> {safetyScore:F1} / 100.");
        }
        else
        {
            notes.Add("Safety Record: Clean incident record (100 / 100).");
        }

        // 4. Training Completion Rate Component (0 - 100)
        var trainingScore = Math.Clamp(input.TrainingCompletionRatePercent, 0.0, 100.0);
        notes.Add($"Training Modules: {trainingScore:F1}% completed ({trainingScore:F1} / 100).");

        // Calculate Weighted Sub-Scores
        var totalWeight = _weights.CertificationsWeight +
                          _weights.ExperienceWeight +
                          _weights.SafetyRecordWeight +
                          _weights.TrainingWeight;

        if (totalWeight <= 0) totalWeight = 1.0;

        var weightedCert = certsScore * (_weights.CertificationsWeight / totalWeight);
        var weightedExp = expScore * (_weights.ExperienceWeight / totalWeight);
        var weightedSafety = safetyScore * (_weights.SafetyRecordWeight / totalWeight);
        var weightedTraining = trainingScore * (_weights.TrainingWeight / totalWeight);

        var overallRaw = weightedCert + weightedExp + weightedSafety + weightedTraining;
        var finalScore = Math.Round(Math.Clamp(overallRaw, 0.0, 100.0), 1);

        var level = DetermineCompetencyLevel(finalScore, input.SafetyIncidentsCount);
        var isEligible = finalScore >= _weights.MinimumPermitApprovalScore && certsScore > 0;

        if (!isEligible)
        {
            if (certsScore <= 0)
                notes.Add("Eligibility Warning: Worker has no valid safety certifications on file.");
            if (finalScore < _weights.MinimumPermitApprovalScore)
                notes.Add($"Eligibility Warning: Total competency score ({finalScore:F1}) is below minimum site threshold ({_weights.MinimumPermitApprovalScore:F1}).");
        }

        return new CompetencyScore(
            OverallScore: finalScore,
            CompetencyLevel: level,
            IsPermitEligible: isEligible,
            CertificationsScore: Math.Round(certsScore, 1),
            ExperienceScore: Math.Round(expScore, 1),
            SafetyRecordScore: Math.Round(safetyScore, 1),
            TrainingScore: Math.Round(trainingScore, 1),
            WeightedCertificationsScore: Math.Round(weightedCert, 1),
            WeightedExperienceScore: Math.Round(weightedExp, 1),
            WeightedSafetyScore: Math.Round(weightedSafety, 1),
            WeightedTrainingScore: Math.Round(weightedTraining, 1),
            BreakdownNotes: notes
        );
    }

    /// <inheritdoc />
    public CompetencyScore ScoreWorker(
        Worker worker,
        int yearsOfExperience,
        int safetyIncidentCount,
        double trainingCompletionRate)
    {
        ArgumentNullException.ThrowIfNull(worker);

        var now = DateTime.UtcNow;
        var validCertsCount = worker.Certificates.Count(c =>
            c.Status == CertificateStatus.Valid &&
            c.ExpiryDate > now);

        var fullName = $"{worker.FirstName} {worker.LastName}".Trim();

        var input = new WorkerCompetencyInput(
            WorkerId: worker.Id,
            WorkerName: string.IsNullOrWhiteSpace(fullName) ? worker.BadgeNumber : fullName,
            ValidCertificatesCount: validCertsCount,
            YearsOfExperience: yearsOfExperience,
            SafetyIncidentsCount: safetyIncidentCount,
            TrainingCompletionRatePercent: trainingCompletionRate
        );

        return ScoreWorker(input);
    }

    private double CalculateCertificationsScore(int count)
    {
        if (count <= 0) return 0.0;
        var ratio = (double)count / Math.Max(1, _weights.TargetCertsCount);
        return Math.Min(100.0, ratio * 100.0);
    }

    private double CalculateExperienceScore(int years)
    {
        if (years <= 0) return 0.0;
        var ratio = (double)years / Math.Max(1, _weights.MaxExperienceYearsCap);
        return Math.Min(100.0, ratio * 100.0);
    }

    private double CalculateSafetyScore(int incidents)
    {
        if (incidents <= 0) return 100.0;
        var penalty = incidents * _weights.IncidentPenaltyPoints;
        return Math.Max(0.0, 100.0 - penalty);
    }

    private static string DetermineCompetencyLevel(double score, int safetyIncidents)
    {
        if (safetyIncidents >= 3)
            return "HighRisk";

        return score switch
        {
            >= 85.0 => "Expert",
            >= 70.0 => "Proficient",
            >= 55.0 => "Competent",
            >= 40.0 => "Novice",
            _ => "HighRisk"
        };
    }
}
