using System.Collections.Concurrent;
using ClearToWork.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ClearToWork.Infrastructure.Services;

/// <summary>
/// Audit event types for SIMOPS conflict tracking.
/// </summary>
public enum SimopsAuditEventType
{
    ConflictDetected,
    ResolutionSuggested,
    ResolutionApplied,
    ConflictOverridden,
    ConflictCleared
}

/// <summary>
/// Immutable audit entry capturing SIMOPS collision lifecycle events for statutory and safety compliance.
/// </summary>
public record ConflictAuditRecord(
    Guid AuditId,
    DateTime TimestampUtc,
    SimopsAuditEventType EventType,
    string PrimaryPermitNumber,
    string? ConflictingPermitNumber,
    string ZoneCode,
    string RuleCode,
    HazardSeverity Severity,
    string Message,
    string? PerformedBy,
    IReadOnlyDictionary<string, string>? Metadata = null
);

/// <summary>
/// Contract for logging and auditing SIMOPS (Simultaneous Operations) hazard conflicts and resolution actions.
/// </summary>
public interface IConflictLogger
{
    void LogConflictDetected(
        string primaryPermitNumber,
        string conflictingPermitNumber,
        string zoneCode,
        string ruleCode,
        HazardSeverity severity,
        string explanation,
        DateTime primaryStart,
        DateTime primaryEnd,
        DateTime conflictingStart,
        DateTime conflictingEnd);

    void LogResolutionApplied(
        string primaryPermitNumber,
        string conflictingPermitNumber,
        string resolutionStrategy,
        string details,
        string approvedBy);

    void LogConflictOverridden(
        string primaryPermitNumber,
        string conflictingPermitNumber,
        string reason,
        string authorizedSafetyOfficer,
        HazardSeverity severity);

    void LogConflictCleared(
        string primaryPermitNumber,
        string zoneCode,
        string remarks);

    IReadOnlyList<ConflictAuditRecord> GetAuditTrail(string? permitNumber = null);
    IReadOnlyList<ConflictAuditRecord> GetRecentConflicts(int limit = 50);
}

/// <summary>
/// Logging helper and in-memory audit store for SIMOPS clash detection in ClearToWork AI.
/// Produces structured Serilog/Console logs via <see cref="ILogger{ConflictLogger}"/> and
/// preserves an auditable event ledger for safety incident investigations.
/// </summary>
public class ConflictLogger : IConflictLogger
{
    private readonly ILogger<ConflictLogger> _logger;
    private readonly ConcurrentQueue<ConflictAuditRecord> _auditLog = new();
    private const int MaxAuditLogCapacity = 1000;

    public ConflictLogger(ILogger<ConflictLogger> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs when an incompatible SIMOPS clash is discovered during permit validation or scheduling.
    /// </summary>
    public void LogConflictDetected(
        string primaryPermitNumber,
        string conflictingPermitNumber,
        string zoneCode,
        string ruleCode,
        HazardSeverity severity,
        string explanation,
        DateTime primaryStart,
        DateTime primaryEnd,
        DateTime conflictingStart,
        DateTime conflictingEnd)
    {
        _logger.LogWarning(
            "[SIMOPS CONFLICT DETECTED] Permit {PrimaryPermit} clashes with {ConflictingPermit} in Zone {ZoneCode}. " +
            "Rule: {RuleCode} | Severity: {Severity} | Primary Window: {PrimaryStart:HH:mm}-{PrimaryEnd:HH:mm} | " +
            "Conflicting Window: {ConflictingStart:HH:mm}-{ConflictingEnd:HH:mm} | Reason: {Explanation}",
            primaryPermitNumber,
            conflictingPermitNumber,
            zoneCode,
            ruleCode,
            severity,
            primaryStart,
            primaryEnd,
            conflictingStart,
            conflictingEnd,
            explanation);

        var metadata = new Dictionary<string, string>
        {
            ["PrimaryWindow"] = $"{primaryStart:s} to {primaryEnd:s}",
            ["ConflictingWindow"] = $"{conflictingStart:s} to {conflictingEnd:s}",
            ["RuleCode"] = ruleCode
        };

        AppendAuditRecord(new ConflictAuditRecord(
            AuditId: Guid.NewGuid(),
            TimestampUtc: DateTime.UtcNow,
            EventType: SimopsAuditEventType.ConflictDetected,
            PrimaryPermitNumber: primaryPermitNumber,
            ConflictingPermitNumber: conflictingPermitNumber,
            ZoneCode: zoneCode,
            RuleCode: ruleCode,
            Severity: severity,
            Message: explanation,
            PerformedBy: "System.SIMOPS.Engine",
            Metadata: metadata
        ));
    }

    /// <summary>
    /// Records an agreed SIMOPS conflict resolution (e.g. time shift, barricading, or sequence change).
    /// </summary>
    public void LogResolutionApplied(
        string primaryPermitNumber,
        string conflictingPermitNumber,
        string resolutionStrategy,
        string details,
        string approvedBy)
    {
        _logger.LogInformation(
            "[SIMOPS RESOLUTION APPLIED] Strategy '{Strategy}' accepted for conflict between {PrimaryPermit} and {ConflictingPermit}. " +
            "Approved by: {ApprovedBy}. Details: {Details}",
            resolutionStrategy,
            primaryPermitNumber,
            conflictingPermitNumber,
            approvedBy,
            details);

        AppendAuditRecord(new ConflictAuditRecord(
            AuditId: Guid.NewGuid(),
            TimestampUtc: DateTime.UtcNow,
            EventType: SimopsAuditEventType.ResolutionApplied,
            PrimaryPermitNumber: primaryPermitNumber,
            ConflictingPermitNumber: conflictingPermitNumber,
            ZoneCode: "N/A",
            RuleCode: "RES-" + resolutionStrategy.ToUpperInvariant(),
            Severity: HazardSeverity.Low,
            Message: $"Resolution [{resolutionStrategy}]: {details}",
            PerformedBy: approvedBy
        ));
    }

    /// <summary>
    /// Audits manual override by authorized safety personnel (e.g. Area Authority accepting calculated residual risk).
    /// </summary>
    public void LogConflictOverridden(
        string primaryPermitNumber,
        string conflictingPermitNumber,
        string reason,
        string authorizedSafetyOfficer,
        HazardSeverity severity)
    {
        _logger.LogCritical(
            "[SIMOPS CONFLICT OVERRIDE] Safety Officer {Officer} manually overrode conflict between {PrimaryPermit} and {ConflictingPermit}. " +
            "Original Severity: {Severity}. Formal Justification: {Reason}",
            authorizedSafetyOfficer,
            primaryPermitNumber,
            conflictingPermitNumber,
            severity,
            reason);

        AppendAuditRecord(new ConflictAuditRecord(
            AuditId: Guid.NewGuid(),
            TimestampUtc: DateTime.UtcNow,
            EventType: SimopsAuditEventType.ConflictOverridden,
            PrimaryPermitNumber: primaryPermitNumber,
            ConflictingPermitNumber: conflictingPermitNumber,
            ZoneCode: "N/A",
            RuleCode: "MANUAL_OVERRIDE",
            Severity: severity,
            Message: $"Safety Officer Override: {reason}",
            PerformedBy: authorizedSafetyOfficer
        ));
    }

    /// <summary>
    /// Records when an active zone conflict is cleared due to completion or cancellation of one permit.
    /// </summary>
    public void LogConflictCleared(string primaryPermitNumber, string zoneCode, string remarks)
    {
        _logger.LogInformation(
            "[SIMOPS CONFLICT CLEARED] Permit {PrimaryPermit} conflict in Zone {ZoneCode} is now resolved. Remarks: {Remarks}",
            primaryPermitNumber,
            zoneCode,
            remarks);

        AppendAuditRecord(new ConflictAuditRecord(
            AuditId: Guid.NewGuid(),
            TimestampUtc: DateTime.UtcNow,
            EventType: SimopsAuditEventType.ConflictCleared,
            PrimaryPermitNumber: primaryPermitNumber,
            ConflictingPermitNumber: null,
            ZoneCode: zoneCode,
            RuleCode: "CLEARED",
            Severity: HazardSeverity.None,
            Message: remarks,
            PerformedBy: "System.LifecycleMonitor"
        ));
    }

    /// <summary>
    /// Retrieves historical audit trail entries, optionally filtered by permit number.
    /// </summary>
    public IReadOnlyList<ConflictAuditRecord> GetAuditTrail(string? permitNumber = null)
    {
        var records = _auditLog.ToArray();
        if (string.IsNullOrWhiteSpace(permitNumber))
        {
            return records.OrderByDescending(r => r.TimestampUtc).ToList();
        }

        return records
            .Where(r => string.Equals(r.PrimaryPermitNumber, permitNumber, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(r.ConflictingPermitNumber, permitNumber, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.TimestampUtc)
            .ToList();
    }

    /// <summary>
    /// Retrieves the most recent conflict records.
    /// </summary>
    public IReadOnlyList<ConflictAuditRecord> GetRecentConflicts(int limit = 50)
    {
        return _auditLog.ToArray()
            .Where(r => r.EventType == SimopsAuditEventType.ConflictDetected)
            .OrderByDescending(r => r.TimestampUtc)
            .Take(Math.Max(1, limit))
            .ToList();
    }

    private void AppendAuditRecord(ConflictAuditRecord record)
    {
        _auditLog.Enqueue(record);

        // Keep memory bounded to MaxAuditLogCapacity
        while (_auditLog.Count > MaxAuditLogCapacity && _auditLog.TryDequeue(out _))
        {
        }
    }
}
