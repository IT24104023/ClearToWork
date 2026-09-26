using System.Text.Json.Serialization;

namespace ClearToWork.Infrastructure.Services;

#region Domain Enums & Models

/// <summary>
/// Energy sources isolated under Lockout/Tagout (LOTO) protocols to prevent hazardous energy release.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IsolationType
{
    /// <summary>
    /// High/medium/low voltage electrical energy isolated via circuit breakers, disconnect switches, or racked-out contactors.
    /// </summary>
    Electrical = 1,

    /// <summary>
    /// Process piping fluid flow isolated via ball, gate, or globe valves in closed position.
    /// </summary>
    MechanicalValve = 2,

    /// <summary>
    /// Pressurized hydraulic fluid isolated via accumulator block-and-bleed assemblies.
    /// </summary>
    Hydraulic = 3,

    /// <summary>
    /// Compressed instrument or utility air isolated via pneumatic shutoff valves and bled down.
    /// </summary>
    Pneumatic = 4,

    /// <summary>
    /// Positive physical separation of hazardous hydrocarbon or toxic chemicals via spectacle blind or blind flange insertion.
    /// </summary>
    ChemicalBlind = 5,

    /// <summary>
    /// Thermal energy isolated from steam, hot oil, or cryogenic systems.
    /// </summary>
    Thermal = 6
}

/// <summary>
/// Physical Lockout/Tagout state progression for equipment isolation points.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IsolationState
{
    /// <summary>
    /// Isolation point is in normal operational configuration; energy flow is unrestricted.
    /// </summary>
    OpenDeIsolated = 1,

    /// <summary>
    /// Physical padlock and lockout hasp applied to the isolation device in the safe/off position.
    /// </summary>
    LockedIsolated = 2,

    /// <summary>
    /// Physical Danger tag applied indicating isolation, but waiting for padlock application.
    /// </summary>
    TaggedOut = 3,

    /// <summary>
    /// Zero residual energy (voltage test, pressure gauge bleed, mechanical try-step) physically verified and signed off.
    /// </summary>
    VerifiedZeroEnergy = 4
}

/// <summary>
/// Overall compliance verdict for a Permit-to-Work LOTO package.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LotoComplianceStatus
{
    /// <summary>
    /// All isolation points locked, zero-energy verified, lock ownership valid, and zero orphaned locks.
    /// </summary>
    FullyCompliant = 1,

    /// <summary>
    /// Non-critical observations or advisory warnings detected; permit activation requires supervisor review.
    /// </summary>
    ActionRequired = 2,

    /// <summary>
    /// Critical safety violation detected (e.g., unlocked isolation point, zero energy not verified, unauthorized lock). Permit activation is strictly blocked.
    /// </summary>
    CriticalBreach = 3
}

/// <summary>
/// Severity level associated with LOTO discrepancies and safety infractions.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LotoViolationSeverity
{
    /// <summary>
    /// Informational note regarding documentation, tagging clarity, or badge synchronization.
    /// </summary>
    Advisory = 1,

    /// <summary>
    /// Procedural discrepancy requiring corrective action prior to permit hand-back.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// High-risk non-compliance that blocks permit activation or field work execution.
    /// </summary>
    Critical = 3,

    /// <summary>
    /// Severe safety breach triggering an immediate stop-work order and incident notification.
    /// </summary>
    ImmediateStopWork = 4
}

/// <summary>
/// Physical safety padlock attached to an isolation point hasp.
/// Follows OSHA 1910.147 "One Person, One Lock, One Key" rule.
/// </summary>
public record LotoLock(
    Guid LockId,
    string LockSerialNumber,
    Guid WorkerId,
    string WorkerName,
    string WorkerBadge,
    DateTime AppliedAtUtc,
    string? LockColor = "Red"
);

/// <summary>
/// Physical energy isolation point (breaker, block valve, spectacle blind) tracked in the facility.
/// </summary>
public class IsolationPoint
{
    /// <summary>
    /// Unique identifier for the isolation point entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Asset tag number engraved on the field isolation device (e.g., "MOV-401-A", "SWGR-02-BKR-14").
    /// </summary>
    public string TagNumber { get; set; } = string.Empty;

    /// <summary>
    /// Functional engineering description of the isolation point.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Plant operational zone where this physical asset resides.
    /// </summary>
    public string ZoneCode { get; set; } = string.Empty;

    /// <summary>
    /// Energy category isolated by this device.
    /// </summary>
    public IsolationType Type { get; set; }

    /// <summary>
    /// Current physical LOTO state.
    /// </summary>
    public IsolationState State { get; set; } = IsolationState.OpenDeIsolated;

    /// <summary>
    /// Permit ID currently governing this isolation point, if actively isolated.
    /// </summary>
    public Guid? ActivePermitId { get; set; }

    /// <summary>
    /// Collection of personal safety padlocks applied to this isolation point hasp.
    /// </summary>
    public List<LotoLock> AppliedLocks { get; set; } = new();

    /// <summary>
    /// Indicates whether zero residual energy verification (try-step, bleed, test) has been conducted.
    /// </summary>
    public bool ZeroEnergyVerified { get; set; }

    /// <summary>
    /// Badge number of the authorized isolation authority who verified zero energy state.
    /// </summary>
    public string? ZeroEnergyVerifiedBy { get; set; }

    /// <summary>
    /// Timestamp when zero energy state was verified.
    /// </summary>
    public DateTime? ZeroEnergyVerifiedAt { get; set; }
}

/// <summary>
/// Real-time site access and gate presence tracking for a worker.
/// </summary>
public record WorkerSitePresence(
    Guid WorkerId,
    string WorkerBadge,
    string FullName,
    bool IsCurrentlyOnSite,
    DateTime? LastBadgeInUtc,
    DateTime? LastBadgeOutUtc,
    string? CurrentZone = null
);

/// <summary>
/// Operational context and boundary requirements for an active or impending Permit-to-Work.
/// </summary>
public record PermitLotoContext(
    Guid PermitId,
    string PermitNumber,
    string PermitTitle,
    string WorkZoneCode,
    IReadOnlyList<Guid> RequiredIsolationPointIds,
    IReadOnlyList<Guid> AssignedWorkerIds,
    Guid? LeadTechnicianWorkerId = null,
    string? LeadTechnicianBadge = null
);

/// <summary>
/// Discrepancy or safety rule violation detected during LOTO verification.
/// </summary>
public record LotoViolation(
    string ViolationCode,
    LotoViolationSeverity Severity,
    string Description,
    string? IsolationPointTag,
    string? WorkerBadge,
    string RemediationAction
);

/// <summary>
/// Detailed record of a safety padlock whose owner is no longer on facility grounds.
/// </summary>
public record OrphanedLockDetail(
    Guid IsolationPointId,
    string IsolationPointTag,
    string LockSerialNumber,
    Guid WorkerId,
    string WorkerName,
    string WorkerBadge,
    DateTime? WorkerDepartedUtc,
    TimeSpan DurationOrphaned,
    string RecommendedProcedure
);

/// <summary>
/// Evaluation result of pre-activation verification for a Permit-to-Work.
/// </summary>
public record PermitActivationCheckResult(
    Guid PermitId,
    string PermitNumber,
    bool IsApprovedForActivation,
    int TotalRequiredPoints,
    int LockedPointsCount,
    int VerifiedZeroEnergyCount,
    bool AllPointsLocked,
    bool AllPointsZeroEnergyVerified,
    bool LockOwnershipValid,
    bool HasOrphanedLocks,
    IReadOnlyList<LotoViolation> Violations,
    string Summary
);

/// <summary>
/// Full audit compliance report for facility LOTO management and regulatory reporting.
/// </summary>
public record LotoComplianceReport(
    Guid ReportId,
    Guid PermitId,
    string PermitNumber,
    LotoComplianceStatus Status,
    bool CanActivatePermit,
    int TotalRequiredIsolationPoints,
    int CompliantPointsCount,
    int TotalLocksApplied,
    IReadOnlyList<LotoViolation> Violations,
    IReadOnlyList<OrphanedLockDetail> OrphanedLocks,
    IReadOnlyList<IsolationPoint> IsolationPointSnapshots,
    DateTime EvaluatedAtUtc,
    string EvaluatedBy,
    string SummaryNotes
);

#endregion

#region Service Contract

/// <summary>
/// Service interface governing Lockout/Tagout (LOTO) compliance checks in ClearToWork AI.
/// Validates energy isolation completeness before permit activation, enforces personal lock ownership rules,
/// detects orphaned locks from departed workers, and produces comprehensive safety audit reports.
/// </summary>
public interface ILOTOComplianceChecker
{
    /// <summary>
    /// Verifies whether all required isolation points for a permit are locked, tagged, and zero-energy verified
    /// prior to authorizing permit activation.
    /// </summary>
    /// <param name="permit">The permit LOTO context containing required isolation points and assigned workers.</param>
    /// <param name="facilityPoints">All isolation points registered in the facility or relevant unit.</param>
    /// <param name="workerPresences">Current site gate presence records for facility personnel.</param>
    /// <returns>A structured <see cref="PermitActivationCheckResult"/> indicating pass/fail status and violations.</returns>
    PermitActivationCheckResult VerifyPreActivationCompliance(
        PermitLotoContext permit,
        IEnumerable<IsolationPoint> facilityPoints,
        IEnumerable<WorkerSitePresence> workerPresences);

    /// <summary>
    /// Validates that all physical padlocks attached to an isolation point belong exclusively to workers
    /// assigned to the authorized permit work party or designated lead isolation authority.
    /// </summary>
    /// <param name="point">The isolation point being audited.</param>
    /// <param name="assignedWorkerIds">The worker IDs officially assigned to the permit work team.</param>
    /// <param name="leadTechnicianWorkerId">Optional worker ID of the lead isolation technician or performing authority.</param>
    /// <returns>A collection of ownership violations, or empty if all locks are legitimately owned.</returns>
    IReadOnlyList<LotoViolation> ValidateLockOwnership(
        IsolationPoint point,
        IEnumerable<Guid> assignedWorkerIds,
        Guid? leadTechnicianWorkerId = null);

    /// <summary>
    /// Scans isolation points to identify orphaned padlocks where the worker who applied the lock has badged out
    /// or departed the site while their personal padlock remains attached to an active isolation hasp.
    /// </summary>
    /// <param name="isolationPoints">The isolation points to examine.</param>
    /// <param name="workerPresences">The real-time site presence registry.</param>
    /// <param name="asOfUtc">Audit reference timestamp; defaults to UTC now.</param>
    /// <returns>A list of detected orphaned lock details.</returns>
    IReadOnlyList<OrphanedLockDetail> DetectOrphanedLocks(
        IEnumerable<IsolationPoint> isolationPoints,
        IEnumerable<WorkerSitePresence> workerPresences,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Generates a formal LOTO compliance audit report for a permit, detailing isolation point statuses,
    /// ownership integrity, zero-energy confirmations, and regulatory remediation actions.
    /// </summary>
    /// <param name="permit">The permit LOTO context.</param>
    /// <param name="facilityPoints">Available isolation points.</param>
    /// <param name="workerPresences">Current site gate presence data.</param>
    /// <param name="auditorBadge">Badge number of the safety auditor or automated engine.</param>
    /// <param name="asOfUtc">Evaluation timestamp; defaults to UTC now.</param>
    /// <returns>A complete <see cref="LotoComplianceReport"/>.</returns>
    LotoComplianceReport GenerateComplianceReport(
        PermitLotoContext permit,
        IEnumerable<IsolationPoint> facilityPoints,
        IEnumerable<WorkerSitePresence> workerPresences,
        string auditorBadge = "SYSTEM_SAFETY_ENGINE",
        DateTime? asOfUtc = null);

    /// <summary>
    /// Quick validation check indicating whether a permit is clear to transition to Active status under LOTO rules.
    /// </summary>
    /// <param name="permit">The permit LOTO context.</param>
    /// <param name="facilityPoints">Available isolation points.</param>
    /// <param name="workerPresences">Current site presence data.</param>
    /// <returns>True if 100% compliant and ready for activation; otherwise false.</returns>
    bool IsPermitReadyForActivation(
        PermitLotoContext permit,
        IEnumerable<IsolationPoint> facilityPoints,
        IEnumerable<WorkerSitePresence> workerPresences);
}

#endregion

#region Implementation

/// <summary>
/// Production implementation of <see cref="ILOTOComplianceChecker"/> enforcing OSHA 29 CFR 1910.147
/// (Control of Hazardous Energy) and IOGP Life-Saving Rules for Permit-to-Work energy isolations.
/// </summary>
public class LOTOComplianceChecker : ILOTOComplianceChecker
{
    /// <inheritdoc />
    public PermitActivationCheckResult VerifyPreActivationCompliance(
        PermitLotoContext permit,
        IEnumerable<IsolationPoint> facilityPoints,
        IEnumerable<WorkerSitePresence> workerPresences)
    {
        ArgumentNullException.ThrowIfNull(permit);
        ArgumentNullException.ThrowIfNull(facilityPoints);
        ArgumentNullException.ThrowIfNull(workerPresences);

        var facilityPointMap = facilityPoints.ToDictionary(p => p.Id);
        var presenceMap = workerPresences.ToDictionary(p => p.WorkerId);
        var violations = new List<LotoViolation>();

        var requiredIds = permit.RequiredIsolationPointIds ?? Array.Empty<Guid>();
        var totalRequired = requiredIds.Count;
        var lockedCount = 0;
        var zeroEnergyVerifiedCount = 0;

        if (totalRequired == 0)
        {
            violations.Add(new LotoViolation(
                ViolationCode: "LOTO-ERR-001",
                Severity: LotoViolationSeverity.Warning,
                Description: $"Permit {permit.PermitNumber} has no required isolation points defined. If work involves hazardous energy, an isolation plan must be established.",
                IsolationPointTag: null,
                WorkerBadge: permit.LeadTechnicianBadge,
                RemediationAction: "Verify whether mechanical, electrical, or chemical energy isolation is required for this work scope."
            ));
        }

        var matchedPoints = new List<IsolationPoint>();

        foreach (var reqId in requiredIds)
        {
            if (!facilityPointMap.TryGetValue(reqId, out var point))
            {
                violations.Add(new LotoViolation(
                    ViolationCode: "LOTO-CRIT-002",
                    Severity: LotoViolationSeverity.Critical,
                    Description: $"Required isolation point ID '{reqId}' does not exist in facility equipment records.",
                    IsolationPointTag: reqId.ToString(),
                    WorkerBadge: null,
                    RemediationAction: "Correct isolation certificate configuration to reference valid plant asset tags."
                ));
                continue;
            }

            matchedPoints.Add(point);

            // Check 1: Physical Lock State
            var isLocked = point.State == IsolationState.LockedIsolated || point.State == IsolationState.VerifiedZeroEnergy;
            if (isLocked)
            {
                lockedCount++;
            }
            else
            {
                violations.Add(new LotoViolation(
                    ViolationCode: "LOTO-CRIT-003",
                    Severity: LotoViolationSeverity.Critical,
                    Description: $"Isolation point '{point.TagNumber}' ({point.Type}) is NOT locked (Current State: {point.State}).",
                    IsolationPointTag: point.TagNumber,
                    WorkerBadge: null,
                    RemediationAction: $"Apply physical padlock and Danger Tag to {point.TagNumber} before permit release."
                ));
            }

            // Check 2: Applied Padlocks Count
            if (point.AppliedLocks.Count == 0)
            {
                violations.Add(new LotoViolation(
                    ViolationCode: "LOTO-CRIT-004",
                    Severity: LotoViolationSeverity.Critical,
                    Description: $"Isolation point '{point.TagNumber}' has zero applied padlocks on the lockout hasp.",
                    IsolationPointTag: point.TagNumber,
                    WorkerBadge: null,
                    RemediationAction: $"Work party members must affix individual personal safety locks to {point.TagNumber}."
                ));
            }

            // Check 3: Cross-Permit Lock Clash
            if (point.ActivePermitId.HasValue && point.ActivePermitId.Value != permit.PermitId)
            {
                violations.Add(new LotoViolation(
                    ViolationCode: "LOTO-CRIT-005",
                    Severity: LotoViolationSeverity.Critical,
                    Description: $"Isolation point '{point.TagNumber}' is locked under conflicting Permit ID '{point.ActivePermitId.Value}'. Shared isolation procedure must be applied.",
                    IsolationPointTag: point.TagNumber,
                    WorkerBadge: null,
                    RemediationAction: "Coordinate with Area Authority to apply group lockout lockbox protocol."
                ));
            }

            // Check 4: Zero Energy Verification Sign-Off
            if (point.ZeroEnergyVerified && !string.IsNullOrWhiteSpace(point.ZeroEnergyVerifiedBy))
            {
                zeroEnergyVerifiedCount++;
            }
            else
            {
                violations.Add(new LotoViolation(
                    ViolationCode: "LOTO-CRIT-006",
                    Severity: LotoViolationSeverity.Critical,
                    Description: $"Zero energy verification has not been signed off for isolation point '{point.TagNumber}'.",
                    IsolationPointTag: point.TagNumber,
                    WorkerBadge: null,
                    RemediationAction: $"Conduct physical test-for-dead/pressure bleed on {point.TagNumber} and record verification signature."
                ));
            }

            // Check 5: Lock Ownership Validation
            var ownershipViolations = ValidateLockOwnership(point, permit.AssignedWorkerIds, permit.LeadTechnicianWorkerId);
            violations.AddRange(ownershipViolations);
        }

        // Check 6: Orphaned Locks Detection across required points
        var orphanedLocks = DetectOrphanedLocks(matchedPoints, workerPresences);
        if (orphanedLocks.Count > 0)
        {
            foreach (var orphaned in orphanedLocks)
            {
                violations.Add(new LotoViolation(
                    ViolationCode: "LOTO-CRIT-007",
                    Severity: LotoViolationSeverity.Critical,
                    Description: $"Orphaned lock '{orphaned.LockSerialNumber}' detected on '{orphaned.IsolationPointTag}'. Worker '{orphaned.WorkerName}' ({orphaned.WorkerBadge}) departed site {orphaned.DurationOrphaned.TotalHours:F1}h ago.",
                    IsolationPointTag: orphaned.IsolationPointTag,
                    WorkerBadge: orphaned.WorkerBadge,
                    RemediationAction: orphaned.RecommendedProcedure
                ));
            }
        }

        var allPointsLocked = totalRequired > 0 && lockedCount == totalRequired;
        var allZeroEnergyVerified = totalRequired > 0 && zeroEnergyVerifiedCount == totalRequired;
        var lockOwnershipValid = !violations.Any(v => v.ViolationCode == "LOTO-OWN-001");
        var hasOrphanedLocks = orphanedLocks.Count > 0;

        var isApproved = allPointsLocked &&
                         allZeroEnergyVerified &&
                         lockOwnershipValid &&
                         !hasOrphanedLocks &&
                         !violations.Any(v => v.Severity is LotoViolationSeverity.Critical or LotoViolationSeverity.ImmediateStopWork);

        var summary = isApproved
            ? $"LOTO VERIFICATION PASSED: All {totalRequired} required isolation points are securely locked, zero-energy verified, and compliant. Permit is cleared for activation."
            : $"LOTO VERIFICATION FAILED: Permit activation blocked due to {violations.Count(v => v.Severity >= LotoViolationSeverity.Critical)} critical isolation non-compliance(s).";

        return new PermitActivationCheckResult(
            PermitId: permit.PermitId,
            PermitNumber: permit.PermitNumber,
            IsApprovedForActivation: isApproved,
            TotalRequiredPoints: totalRequired,
            LockedPointsCount: lockedCount,
            VerifiedZeroEnergyCount: zeroEnergyVerifiedCount,
            AllPointsLocked: allPointsLocked,
            AllPointsZeroEnergyVerified: allZeroEnergyVerified,
            LockOwnershipValid: lockOwnershipValid,
            HasOrphanedLocks: hasOrphanedLocks,
            Violations: violations,
            Summary: summary
        );
    }

    /// <inheritdoc />
    public IReadOnlyList<LotoViolation> ValidateLockOwnership(
        IsolationPoint point,
        IEnumerable<Guid> assignedWorkerIds,
        Guid? leadTechnicianWorkerId = null)
    {
        ArgumentNullException.ThrowIfNull(point);
        ArgumentNullException.ThrowIfNull(assignedWorkerIds);

        var authorizedIds = new HashSet<Guid>(assignedWorkerIds);
        if (leadTechnicianWorkerId.HasValue)
        {
            authorizedIds.Add(leadTechnicianWorkerId.Value);
        }

        var violations = new List<LotoViolation>();

        foreach (var lockItem in point.AppliedLocks)
        {
            if (!authorizedIds.Contains(lockItem.WorkerId))
            {
                violations.Add(new LotoViolation(
                    ViolationCode: "LOTO-OWN-001",
                    Severity: LotoViolationSeverity.Critical,
                    Description: $"Unauthorized Lock: Lock '{lockItem.LockSerialNumber}' on '{point.TagNumber}' is owned by '{lockItem.WorkerName}' (Badge: {lockItem.WorkerBadge}), who is not assigned to this permit.",
                    IsolationPointTag: point.TagNumber,
                    WorkerBadge: lockItem.WorkerBadge,
                    RemediationAction: "Worker must either be added to permit work party or personal lock must be replaced by an assigned worker."
                ));
            }
        }

        // Check for duplicate lock serial numbers applied to the same point
        var duplicates = point.AppliedLocks
            .GroupBy(l => l.LockSerialNumber.Trim().ToUpperInvariant())
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var dup in duplicates)
        {
            violations.Add(new LotoViolation(
                ViolationCode: "LOTO-OWN-002",
                Severity: LotoViolationSeverity.Critical,
                Description: $"Duplicate Lock Serial: Serial '{dup.Key}' appears {dup.Count()} times on '{point.TagNumber}'. Lock serials must be unique.",
                IsolationPointTag: point.TagNumber,
                WorkerBadge: null,
                RemediationAction: "Inspect physical lock keying; duplicate serials indicate key duplication risk."
            ));
        }

        return violations;
    }

    /// <inheritdoc />
    public IReadOnlyList<OrphanedLockDetail> DetectOrphanedLocks(
        IEnumerable<IsolationPoint> isolationPoints,
        IEnumerable<WorkerSitePresence> workerPresences,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(isolationPoints);
        ArgumentNullException.ThrowIfNull(workerPresences);

        var now = asOfUtc ?? DateTime.UtcNow;
        var presenceMap = workerPresences.ToDictionary(p => p.WorkerId);
        var orphanedLocks = new List<OrphanedLockDetail>();

        foreach (var point in isolationPoints)
        {
            foreach (var lockItem in point.AppliedLocks)
            {
                if (presenceMap.TryGetValue(lockItem.WorkerId, out var presence))
                {
                    // Check if worker has departed the site
                    if (!presence.IsCurrentlyOnSite)
                    {
                        var departedAt = presence.LastBadgeOutUtc ?? lockItem.AppliedAtUtc;
                        var duration = now > departedAt ? now - departedAt : TimeSpan.Zero;

                        orphanedLocks.Add(new OrphanedLockDetail(
                            IsolationPointId: point.Id,
                            IsolationPointTag: point.TagNumber,
                            LockSerialNumber: lockItem.LockSerialNumber,
                            WorkerId: lockItem.WorkerId,
                            WorkerName: lockItem.WorkerName,
                            WorkerBadge: lockItem.WorkerBadge,
                            WorkerDepartedUtc: presence.LastBadgeOutUtc,
                            DurationOrphaned: duration,
                            RecommendedProcedure: $"Execute OSHA 1910.147(e)(3) Abandoned Lock Removal: Verify worker {lockItem.WorkerBadge} is offsite, notify worker, obtain Plant Manager sign-off before mechanical removal."
                        ));
                    }
                }
                else
                {
                    // Worker not present in site presence database at all
                    orphanedLocks.Add(new OrphanedLockDetail(
                        IsolationPointId: point.Id,
                        IsolationPointTag: point.TagNumber,
                        LockSerialNumber: lockItem.LockSerialNumber,
                        WorkerId: lockItem.WorkerId,
                        WorkerName: lockItem.WorkerName,
                        WorkerBadge: lockItem.WorkerBadge,
                        WorkerDepartedUtc: null,
                        DurationOrphaned: TimeSpan.Zero,
                        RecommendedProcedure: $"Unregistered Worker Lock: Worker {lockItem.WorkerBadge} has no active site presence record. Verify badge validity with Security."
                    ));
                }
            }
        }

        return orphanedLocks;
    }

    /// <inheritdoc />
    public LotoComplianceReport GenerateComplianceReport(
        PermitLotoContext permit,
        IEnumerable<IsolationPoint> facilityPoints,
        IEnumerable<WorkerSitePresence> workerPresences,
        string auditorBadge = "SYSTEM_SAFETY_ENGINE",
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(permit);

        var now = asOfUtc ?? DateTime.UtcNow;
        var checkResult = VerifyPreActivationCompliance(permit, facilityPoints, workerPresences);
        var pointsList = facilityPoints.Where(p => (permit.RequiredIsolationPointIds ?? Array.Empty<Guid>()).Contains(p.Id)).ToList();
        var orphanedLocks = DetectOrphanedLocks(pointsList, workerPresences, now);

        var totalLocksApplied = pointsList.Sum(p => p.AppliedLocks.Count);
        var compliantPointsCount = pointsList.Count(p =>
            (p.State == IsolationState.LockedIsolated || p.State == IsolationState.VerifiedZeroEnergy) &&
            p.ZeroEnergyVerified &&
            p.AppliedLocks.Count > 0 &&
            !orphanedLocks.Any(o => o.IsolationPointId == p.Id));

        LotoComplianceStatus status;
        if (checkResult.IsApprovedForActivation && checkResult.Violations.Count == 0)
        {
            status = LotoComplianceStatus.FullyCompliant;
        }
        else if (checkResult.Violations.Any(v => v.Severity is LotoViolationSeverity.Critical or LotoViolationSeverity.ImmediateStopWork))
        {
            status = LotoComplianceStatus.CriticalBreach;
        }
        else
        {
            status = LotoComplianceStatus.ActionRequired;
        }

        var notes = status switch
        {
            LotoComplianceStatus.FullyCompliant =>
                $"Permit {permit.PermitNumber} isolation boundary is 100% compliant. All {pointsList.Count} points locked with verified zero energy.",
            LotoComplianceStatus.ActionRequired =>
                $"Permit {permit.PermitNumber} has {checkResult.Violations.Count} advisory observation(s) requiring attention before permit completion.",
            _ =>
                $"CRITICAL STOP WORK: Permit {permit.PermitNumber} has {checkResult.Violations.Count} safety violation(s). Physical work cannot commence until isolations are verified."
        };

        return new LotoComplianceReport(
            ReportId: Guid.NewGuid(),
            PermitId: permit.PermitId,
            PermitNumber: permit.PermitNumber,
            Status: status,
            CanActivatePermit: checkResult.IsApprovedForActivation,
            TotalRequiredIsolationPoints: checkResult.TotalRequiredPoints,
            CompliantPointsCount: compliantPointsCount,
            TotalLocksApplied: totalLocksApplied,
            Violations: checkResult.Violations,
            OrphanedLocks: orphanedLocks,
            IsolationPointSnapshots: pointsList,
            EvaluatedAtUtc: now,
            EvaluatedBy: auditorBadge,
            SummaryNotes: notes
        );
    }

    /// <inheritdoc />
    public bool IsPermitReadyForActivation(
        PermitLotoContext permit,
        IEnumerable<IsolationPoint> facilityPoints,
        IEnumerable<WorkerSitePresence> workerPresences)
    {
        var result = VerifyPreActivationCompliance(permit, facilityPoints, workerPresences);
        return result.IsApprovedForActivation;
    }
}

#endregion
