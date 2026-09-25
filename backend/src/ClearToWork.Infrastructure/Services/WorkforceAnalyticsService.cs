using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Services;

/// <summary>
/// Breakdown of expiring and expired certifications across key monitoring windows.
/// </summary>
public record ExpiringCertificatesBreakdown(
    int Within7Days,
    int Within14Days,
    int Within30Days,
    int Expired
);

/// <summary>
/// Trade-specific workforce metrics.
/// </summary>
public record TradeDistributionItem(
    string Trade,
    int WorkerCount,
    double PercentageOfWorkforce,
    int CompliantCount
);

/// <summary>
/// Contractor company workforce metrics.
/// </summary>
public record CompanyDistributionItem(
    Guid CompanyId,
    string CompanyName,
    int WorkerCount,
    int CompliantCount,
    double ComplianceRate
);

/// <summary>
/// High-level executive and HSE analytics summary of the site workforce.
/// </summary>
public record WorkforceAnalyticsSummary(
    int TotalWorkers,
    int ActiveWorkers,
    int InactiveWorkers,
    Dictionary<string, int> WorkersByTrade,
    Dictionary<string, int> WorkersByCompany,
    double CertificationComplianceRate,
    double AverageCompetencyScore,
    ExpiringCertificatesBreakdown ExpiringCertificates,
    List<TradeDistributionItem> TradeDistribution,
    List<CompanyDistributionItem> CompanyDistribution,
    DateTime GeneratedAtUtc
);

/// <summary>
/// Interface for aggregating workforce metrics, safety compliance rates, and competency trends.
/// </summary>
public interface IWorkforceAnalyticsService
{
    /// <summary>
    /// Generates an analytical summary of site workforce readiness, trade distribution,
    /// company allocations, and certificate expiry risk profiles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A comprehensive <see cref="WorkforceAnalyticsSummary"/>.</returns>
    Task<WorkforceAnalyticsSummary> GetWorkforceAnalyticsSummaryAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of workforce analytics aggregation service for ClearToWork AI.
/// Compiles real-time metrics from the operational database to drive HSE dashboards and audits.
/// </summary>
public class WorkforceAnalyticsService : IWorkforceAnalyticsService
{
    private readonly AppDbContext _context;
    private readonly ICompetencyMatrixScorer _competencyScorer;

    public WorkforceAnalyticsService(
        AppDbContext context,
        ICompetencyMatrixScorer? competencyScorer = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _competencyScorer = competencyScorer ?? new CompetencyMatrixScorer();
    }

    /// <inheritdoc />
    public async Task<WorkforceAnalyticsSummary> GetWorkforceAnalyticsSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Fetch workers including contractor information, certificates, and training records
        var workers = await _context.Workers
            .AsNoTracking()
            .Include(w => w.Contractor)
            .Include(w => w.Certificates)
                .ThenInclude(c => c.CertificateType)
            .Include(w => w.TrainingRecords)
            .ToListAsync(cancellationToken);

        var totalWorkers = workers.Count;
        var activeWorkersList = workers.Where(w => w.IsActive).ToList();
        var activeWorkers = activeWorkersList.Count;
        var inactiveWorkers = totalWorkers - activeWorkers;

        // 1. Total Workers by Trade breakdown
        var workersByTrade = workers
            .GroupBy(w => string.IsNullOrWhiteSpace(w.Trade) ? "Unassigned" : w.Trade)
            .ToDictionary(g => g.Key, g => g.Count());

        // 2. Workers by Company breakdown
        var workersByCompany = workers
            .GroupBy(w => w.Contractor?.CompanyName ?? "Direct Hire / Independent")
            .ToDictionary(g => g.Key, g => g.Count());

        // 3. Compliance and Expiring Certifications tracking
        var allCertificates = workers
            .Where(w => w.IsActive)
            .SelectMany(w => w.Certificates)
            .ToList();

        var expiring7Days = 0;
        var expiring14Days = 0;
        var expiring30Days = 0;
        var expiredCount = 0;

        foreach (var cert in allCertificates)
        {
            var daysRemaining = (cert.ExpiryDate.Date - now.Date).Days;

            if (daysRemaining < 0 || cert.Status == CertificateStatus.Expired)
            {
                expiredCount++;
            }
            else
            {
                if (daysRemaining <= 7) expiring7Days++;
                if (daysRemaining <= 14) expiring14Days++;
                if (daysRemaining <= 30) expiring30Days++;
            }
        }

        var expiringSummary = new ExpiringCertificatesBreakdown(
            Within7Days: expiring7Days,
            Within14Days: expiring14Days,
            Within30Days: expiring30Days,
            Expired: expiredCount
        );

        // 4. Certification Compliance Rate Calculation
        // A worker is compliant if active, possesses at least one certificate, and has zero expired certificates
        var compliantWorkersCount = 0;
        var competencyScores = new List<double>();

        foreach (var worker in activeWorkersList)
        {
            var certs = worker.Certificates.ToList();
            var hasActiveCerts = certs.Count > 0;
            var hasExpiredOrSuspended = certs.Any(c =>
                c.ExpiryDate < now ||
                c.Status == CertificateStatus.Expired ||
                c.Status == CertificateStatus.Suspended);

            var isCompliant = hasActiveCerts && !hasExpiredOrSuspended;
            if (isCompliant)
            {
                compliantWorkersCount++;
            }

            // Calculate competency score
            var validCertsCount = certs.Count(c => c.Status == CertificateStatus.Valid && c.ExpiryDate > now);
            var estimatedExperienceYears = Math.Max(2, (int)(now - worker.CreatedAt).TotalDays / 365 + 3);
            var trainingRate = worker.TrainingRecords.Count > 0 ? 100.0 : 75.0;

            var scoreResult = _competencyScorer.ScoreWorker(
                worker,
                yearsOfExperience: estimatedExperienceYears,
                safetyIncidentCount: 0,
                trainingCompletionRate: trainingRate
            );

            competencyScores.Add(scoreResult.OverallScore);
        }

        var complianceRate = activeWorkers > 0
            ? Math.Round(((double)compliantWorkersCount / activeWorkers) * 100.0, 1)
            : 0.0;

        var averageCompetencyScore = competencyScores.Count > 0
            ? Math.Round(competencyScores.Average(), 1)
            : 0.0;

        // 5. Build Detailed Trade Distribution
        var tradeDistribution = workers
            .GroupBy(w => string.IsNullOrWhiteSpace(w.Trade) ? "Unassigned" : w.Trade)
            .Select(g =>
            {
                var count = g.Count();
                var compliantInTrade = g.Count(w =>
                    w.IsActive &&
                    w.Certificates.Count > 0 &&
                    w.Certificates.All(c => c.ExpiryDate > now && c.Status == CertificateStatus.Valid));

                return new TradeDistributionItem(
                    Trade: g.Key,
                    WorkerCount: count,
                    PercentageOfWorkforce: totalWorkers > 0 ? Math.Round(((double)count / totalWorkers) * 100.0, 1) : 0.0,
                    CompliantCount: compliantInTrade
                );
            })
            .OrderByDescending(t => t.WorkerCount)
            .ToList();

        // 6. Build Detailed Company Distribution
        var companyDistribution = workers
            .GroupBy(w => new
            {
                CompanyId = w.ContractorId,
                CompanyName = w.Contractor?.CompanyName ?? "Direct Hire / Independent"
            })
            .Select(g =>
            {
                var count = g.Count();
                var compliantInCompany = g.Count(w =>
                    w.IsActive &&
                    w.Certificates.Count > 0 &&
                    w.Certificates.All(c => c.ExpiryDate > now && c.Status == CertificateStatus.Valid));

                var rate = count > 0 ? Math.Round(((double)compliantInCompany / count) * 100.0, 1) : 0.0;

                return new CompanyDistributionItem(
                    CompanyId: g.Key.CompanyId,
                    CompanyName: g.Key.CompanyName,
                    WorkerCount: count,
                    CompliantCount: compliantInCompany,
                    ComplianceRate: rate
                );
            })
            .OrderByDescending(c => c.WorkerCount)
            .ToList();

        return new WorkforceAnalyticsSummary(
            TotalWorkers: totalWorkers,
            ActiveWorkers: activeWorkers,
            InactiveWorkers: inactiveWorkers,
            WorkersByTrade: workersByTrade,
            WorkersByCompany: workersByCompany,
            CertificationComplianceRate: complianceRate,
            AverageCompetencyScore: averageCompetencyScore,
            ExpiringCertificates: expiringSummary,
            TradeDistribution: tradeDistribution,
            CompanyDistribution: companyDistribution,
            GeneratedAtUtc: now
        );
    }
}
