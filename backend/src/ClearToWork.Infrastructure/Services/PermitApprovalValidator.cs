using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClearToWork.Infrastructure.Services;

/// <summary>
/// Represents the evaluation outcome of permit approval prerequisites.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indicates whether all mandatory safety prerequisites have been successfully validated.
    /// </summary>
    public bool IsValid => Issues.Count == 0;

    /// <summary>
    /// List of blocking safety issues preventing permit approval.
    /// </summary>
    public List<string> Issues { get; set; } = new();

    /// <summary>
    /// List of non-blocking advisories and caution points for the field team.
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Granular status of each required prerequisite checkpoint.
    /// </summary>
    public Dictionary<string, bool> CheckpointResults { get; set; } = new();

    public static ValidationResult Success(Dictionary<string, bool>? checkpoints = null, List<string>? warnings = null) =>
        new()
        {
            Issues = new List<string>(),
            Warnings = warnings ?? new List<string>(),
            CheckpointResults = checkpoints ?? new Dictionary<string, bool>()
        };

    public static ValidationResult Failure(List<string> issues, Dictionary<string, bool>? checkpoints = null, List<string>? warnings = null) =>
        new()
        {
            Issues = issues,
            Warnings = warnings ?? new List<string>(),
            CheckpointResults = checkpoints ?? new Dictionary<string, bool>()
        };
}

/// <summary>
/// Interface for validating all prerequisites before a permit can transition to Approved status.
/// </summary>
public interface IPermitApprovalValidator
{
    /// <summary>
    /// Validates all mandatory safety prerequisites for the specified permit.
    /// </summary>
    /// <param name="permitId">The unique ID of the permit to inspect.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="ValidationResult"/> detailing whether approval may proceed.</returns>
    Task<ValidationResult> ValidateApprovalPrerequisitesAsync(Guid permitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates all mandatory safety prerequisites for an already-loaded permit instance.
    /// </summary>
    /// <param name="permit">The loaded permit entity graph.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="ValidationResult"/> detailing whether approval may proceed.</returns>
    Task<ValidationResult> ValidateApprovalPrerequisitesAsync(PermitRequest permit, CancellationToken cancellationToken = default);
}

/// <summary>
/// Enforces industrial Permit-to-Work (PTW) approval prerequisites:
/// 1. All mandatory documentation attached (JSA, P&amp;ID isolation diagrams, risk assessment).
/// 2. Authorized Safety Officer sign-off recorded.
/// 3. Hazard assessment and multi-agent AI verification completed.
/// 4. All assigned workforce members hold active, non-expired competency certifications.
/// 5. Zero simultaneous operations (SIMOPS) clashes in target or adjacent hazard zones.
/// </summary>
public class PermitApprovalValidator : IPermitApprovalValidator
{
    private readonly AppDbContext _context;
    private readonly ILogger<PermitApprovalValidator> _logger;

    public PermitApprovalValidator(AppDbContext context, ILogger<PermitApprovalValidator> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ValidationResult> ValidateApprovalPrerequisitesAsync(Guid permitId, CancellationToken cancellationToken = default)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker).ThenInclude(w => w!.Certificates).ThenInclude(c => c.CertificateType)
            .Include(p => p.AssignedAssets).ThenInclude(pa => pa.Asset).ThenInclude(a => a!.InspectionRecords)
            .Include(p => p.EvidencePhotos)
            .Include(p => p.Approvals)
            .Include(p => p.AgentWorkflowRun)
            .FirstOrDefaultAsync(p => p.Id == permitId, cancellationToken);

        if (permit == null)
        {
            return ValidationResult.Failure(new List<string> { $"Permit with ID '{permitId}' was not found in the database." });
        }

        return await ValidateApprovalPrerequisitesAsync(permit, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ValidationResult> ValidateApprovalPrerequisitesAsync(PermitRequest permit, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[PermitValidator] Starting prerequisite validation for Permit {PermitNumber} ({PermitId})",
            permit.PermitNumber, permit.Id);

        var issues = new List<string>();
        var warnings = new List<string>();
        var checkpoints = new Dictionary<string, bool>();

        // -----------------------------------------------------------------------------------------
        // 1. Mandatory Documents Verification
        // -----------------------------------------------------------------------------------------
        bool documentsValid = CheckRequiredDocuments(permit, issues, warnings);
        checkpoints["RequiredDocumentsAttached"] = documentsValid;

        // -----------------------------------------------------------------------------------------
        // 2. Safety Officer Sign-Off Verification
        // -----------------------------------------------------------------------------------------
        bool safetyOfficerSignoffValid = CheckSafetyOfficerSignoff(permit, issues);
        checkpoints["SafetyOfficerSignOffPresent"] = safetyOfficerSignoffValid;

        // -----------------------------------------------------------------------------------------
        // 3. Hazard Assessment Verification
        // -----------------------------------------------------------------------------------------
        bool hazardAssessmentValid = CheckHazardAssessment(permit, issues, warnings);
        checkpoints["HazardAssessmentCompleted"] = hazardAssessmentValid;

        // -----------------------------------------------------------------------------------------
        // 4. Worker Competency & Certification Verification
        // -----------------------------------------------------------------------------------------
        bool workforceValid = CheckWorkerCertifications(permit, issues);
        checkpoints["WorkerCertificationsValid"] = workforceValid;

        // -----------------------------------------------------------------------------------------
        // 5. Active SIMOPS Conflict Verification
        // -----------------------------------------------------------------------------------------
        bool simopsValid = await CheckSimopsConflictsAsync(permit, issues, warnings, cancellationToken);
        checkpoints["NoSimopsConflicts"] = simopsValid;

        bool allValid = issues.Count == 0;
        _logger.LogInformation(
            "[PermitValidator] Prerequisite evaluation completed for Permit {PermitNumber}. Result: {Status} (Issues: {IssueCount}, Warnings: {WarningCount})",
            permit.PermitNumber, allValid ? "PASSED" : "FAILED", issues.Count, warnings.Count);

        return new ValidationResult
        {
            Issues = issues,
            Warnings = warnings,
            CheckpointResults = checkpoints
        };
    }

    /// <summary>
    /// Ensures site documentation (JSA, risk analysis evidence, photos) are attached to the permit.
    /// </summary>
    private static bool CheckRequiredDocuments(PermitRequest permit, List<string> issues, List<string> warnings)
    {
        var submissionEvidence = permit.EvidencePhotos.Where(p => p.Stage == "Submission" || p.Stage == "Planning").ToList();

        if (submissionEvidence.Count == 0)
        {
            issues.Add("Missing required documentation: No Job Safety Analysis (JSA) or site pre-inspection photos attached.");
            return false;
        }

        if (permit.PermitType != null && permit.PermitType.RequiresIsolation && submissionEvidence.Count < 2)
        {
            warnings.Add("High-risk isolation permit requires both P&ID schematics and physical lockout evidence.");
        }

        return true;
    }

    /// <summary>
    /// Verifies that an authorized Safety Officer review decision has been recorded.
    /// </summary>
    private static bool CheckSafetyOfficerSignoff(PermitRequest permit, List<string> issues)
    {
        var validApproval = permit.Approvals.FirstOrDefault(a =>
            a.Decision == DecisionType.Approved &&
            a.SafetyOfficerId != Guid.Empty);

        if (validApproval == null)
        {
            issues.Add("Safety Officer sign-off missing: No approved HSE decision recorded for this permit.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Verifies that a comprehensive hazard assessment / AI safety workflow run has completed cleanly.
    /// </summary>
    private static bool CheckHazardAssessment(PermitRequest permit, List<string> issues, List<string> warnings)
    {
        if (permit.AgentWorkflowRun == null)
        {
            issues.Add("Hazard assessment incomplete: Multi-agent AI safety review workflow has not executed.");
            return false;
        }

        if (permit.AgentWorkflowRun.OutcomeStatus != WorkflowOutcome.Clear)
        {
            issues.Add($"Hazard assessment failed: AI validation reported outcome '{permit.AgentWorkflowRun.OutcomeStatus}'.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(permit.ObjectiveDescription) || permit.ObjectiveDescription.Trim().Length < 15)
        {
            warnings.Add("Objective description is brief. Ensure all task steps and mitigation boundaries are fully detailed.");
        }

        return true;
    }

    /// <summary>
    /// Validates that all workers assigned to this permit possess non-expired, valid certifications.
    /// </summary>
    private static bool CheckWorkerCertifications(PermitRequest permit, List<string> issues)
    {
        if (permit.AssignedWorkers == null || permit.AssignedWorkers.Count == 0)
        {
            issues.Add("Workforce assignment error: At least one qualified worker must be assigned to the permit.");
            return false;
        }

        bool hasWorkerErrors = false;
        var now = DateTime.UtcNow;

        foreach (var pw in permit.AssignedWorkers)
        {
            var worker = pw.Worker;
            if (worker == null) continue;

            if (!worker.IsActive)
            {
                issues.Add($"Worker {worker.BadgeNumber} ({worker.FullName}) is inactive or on safety suspension.");
                hasWorkerErrors = true;
                continue;
            }

            var validCerts = worker.Certificates.Where(c =>
                c.Status == CertificateStatus.Valid &&
                c.ExpiryDate > permit.ScheduledEndTime).ToList();

            if (validCerts.Count == 0)
            {
                issues.Add($"Worker {worker.BadgeNumber} ({worker.FullName}) does not have any active, unexpired certifications covering the permit execution window.");
                hasWorkerErrors = true;
            }
        }

        return !hasWorkerErrors;
    }

    /// <summary>
    /// Identifies active or approved overlapping permits that represent Simultaneous Operations (SIMOPS) risks.
    /// </summary>
    private async Task<bool> CheckSimopsConflictsAsync(
        PermitRequest permit,
        List<string> issues,
        List<string> warnings,
        CancellationToken cancellationToken)
    {
        // 1. Check for overlapping permits in the exact same zone
        var conflictingSameZone = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Where(p => p.Id != permit.Id &&
                        p.ZoneId == permit.ZoneId &&
                        (p.Status == PermitStatus.Active || p.Status == PermitStatus.Approved) &&
                        p.ScheduledStartTime < permit.ScheduledEndTime &&
                        p.ScheduledEndTime > permit.ScheduledStartTime)
            .ToListAsync(cancellationToken);

        if (conflictingSameZone.Count > 0)
        {
            foreach (var conflict in conflictingSameZone)
            {
                issues.Add($"Active SIMOPS clash: Permit {conflict.PermitNumber} ({conflict.PermitType?.Name ?? "Work"}) is concurrently scheduled in the same zone from {conflict.ScheduledStartTime:HH:mm} to {conflict.ScheduledEndTime:HH:mm}.");
            }
            return false;
        }

        // 2. Check adjacent zones for high-risk incompatibilities (e.g. Hot Work next to Solvents/Painting)
        var adjacentZoneIds = await _context.ZoneAdjacencies
            .Where(za => za.ZoneAId == permit.ZoneId)
            .Select(za => za.ZoneBId)
            .ToListAsync(cancellationToken);

        if (adjacentZoneIds.Count > 0)
        {
            var adjacentConflicting = await _context.PermitRequests
                .Include(p => p.PermitType)
                .Where(p => adjacentZoneIds.Contains(p.ZoneId) &&
                            (p.Status == PermitStatus.Active || p.Status == PermitStatus.Approved) &&
                            p.ScheduledStartTime < permit.ScheduledEndTime &&
                            p.ScheduledEndTime > permit.ScheduledStartTime)
                .ToListAsync(cancellationToken);

            foreach (var adj in adjacentConflicting)
            {
                warnings.Add($"Adjacent zone activity: Permit {adj.PermitNumber} ({adj.PermitType?.Name ?? "Work"}) is active concurrently in adjacent zone. Ensure radio coordination is maintained.");
            }
        }

        return true;
    }
}
