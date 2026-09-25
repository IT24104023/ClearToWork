using System.Text.Json.Serialization;

namespace ClearToWork.Infrastructure.Services;

#region Domain Enums & Asset Models

/// <summary>
/// Categories of safety equipment, gas detection monitors, and life-critical tools tracked in the facility crib.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EquipmentType
{
    /// <summary>
    /// Portable or fixed multi-gas / single-gas atmospheric monitors.
    /// </summary>
    GasDetector = 1,

    /// <summary>
    /// Self-Contained Breathing Apparatus (SCBA) or airline emergency escape packs.
    /// </summary>
    BreathingApparatus_SCBA = 2,

    /// <summary>
    /// Heavy-duty safety padlocks and hasps dedicated to Lockout/Tagout energy isolation.
    /// </summary>
    LotoPadlock = 3,

    /// <summary>
    /// Confined space rescue tripod, winch, and retrieval hoisting systems.
    /// </summary>
    ConfinedSpaceTripod = 4,

    /// <summary>
    /// Fall arrest full-body harnesses, shock-absorbing lanyards, and self-retracting lifelines (SRLs).
    /// </summary>
    HarnessLanyard = 5,

    /// <summary>
    /// Certified high/low voltage non-contact and contact test instruments for zero-energy verification.
    /// </summary>
    VoltageDetector = 6,

    /// <summary>
    /// Explosion-proof positive/negative air ventilation blowers and ducting.
    /// </summary>
    VentilationBlower = 7
}

/// <summary>
/// Operational status of equipment assets in the ClearToWork tool crib.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EquipmentStatus
{
    /// <summary>
    /// Certified, inspected, and ready in tool crib for permit checkout.
    /// </summary>
    Available = 1,

    /// <summary>
    /// Actively deployed to an authorized worker under an active Permit-to-Work.
    /// </summary>
    CheckedOut = 2,

    /// <summary>
    /// Under routine maintenance, sensor replacement, or function testing.
    /// </summary>
    InMaintenance = 3,

    /// <summary>
    /// Failed calibration, damaged, or involved in a safety incident; strictly quarantined.
    /// </summary>
    Quarantined = 4,

    /// <summary>
    /// Permanently retired from service.
    /// </summary>
    Decommissioned = 5
}

/// <summary>
/// Operational entity representing a tracked physical asset or safety instrument.
/// </summary>
public class EquipmentItem
{
    /// <summary>
    /// Unique identifier for the equipment asset.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Facility asset barcode or serial tag number (e.g., "BW-ULTRA-001").
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// Manufacturer model description (e.g., "Honeywell BW Ultra 5-Gas").
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Functional equipment category.
    /// </summary>
    public EquipmentType Type { get; set; }

    /// <summary>
    /// Current crib and lifecycle status.
    /// </summary>
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;

    /// <summary>
    /// Date of most recent certified bench calibration.
    /// </summary>
    public DateTime? LastCalibrationDate { get; set; }

    /// <summary>
    /// Expiration deadline for current calibration validity.
    /// </summary>
    public DateTime? NextCalibrationDueDate { get; set; }

    /// <summary>
    /// Cumulative operating or run hours accrued by the equipment.
    /// </summary>
    public double OperatingHours { get; set; }

    /// <summary>
    /// Indicates whether this asset type requires periodic formal calibration.
    /// </summary>
    public bool RequiresCalibration { get; set; }

    /// <summary>
    /// Registration timestamp in UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Permit number under which the equipment is currently active, if checked out.
    /// </summary>
    public string? CurrentAssignedPermit { get; set; }

    /// <summary>
    /// Custodian worker ID currently holding the asset.
    /// </summary>
    public Guid? CurrentAssignedWorkerId { get; set; }
}

/// <summary>
/// Custody checkout and return audit log entry.
/// </summary>
public class EquipmentCheckoutRecord
{
    /// <summary>
    /// Unique identifier for this checkout event.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Equipment item identifier.
    /// </summary>
    public Guid EquipmentId { get; set; }

    /// <summary>
    /// Worker ID receiving custody of the equipment.
    /// </summary>
    public Guid WorkerId { get; set; }

    /// <summary>
    /// Permit number authorizing equipment deployment.
    /// </summary>
    public string PermitNumber { get; set; } = string.Empty;

    /// <summary>
    /// Checkout timestamp in UTC.
    /// </summary>
    public DateTime CheckoutTimeUtc { get; set; }

    /// <summary>
    /// Return timestamp in UTC; null if currently active in the field.
    /// </summary>
    public DateTime? CheckinTimeUtc { get; set; }

    /// <summary>
    /// Hours logged during this specific checkout deployment.
    /// </summary>
    public double HoursLogged { get; set; }

    /// <summary>
    /// Notes regarding physical condition upon return.
    /// </summary>
    public string? ReturnConditionNotes { get; set; }

    /// <summary>
    /// True if the item has been returned and checked back into the tool crib.
    /// </summary>
    public bool IsReturned => CheckinTimeUtc.HasValue;
}

#endregion

#region Analytics DTOs

/// <summary>
/// Inventory and availability breakdown for an individual equipment category.
/// </summary>
public record EquipmentTypeAnalytics(
    EquipmentType Type,
    int TotalCount,
    int AvailableCount,
    int CheckedOutCount,
    int InMaintenanceCount,
    int QuarantinedCount,
    double UtilizationRate
);

/// <summary>
/// Fleet-wide calibration compliance metrics and overdue asset counts.
/// </summary>
public record CalibrationComplianceMetrics(
    int TotalRequiringCalibration,
    int CalibratedAndValidCount,
    int ExpiringSoonCount,
    int OverdueCount,
    double ComplianceRate
);

/// <summary>
/// Equipment fleet utilization statistics.
/// </summary>
public record EquipmentUtilizationMetrics(
    int TotalActiveEquipment,
    int CheckedOutCount,
    double FleetCheckoutUtilizationPercentage,
    double AverageOperatingHoursPerAsset,
    double TotalOperatingHoursLogged
);

/// <summary>
/// Lockout/Tagout (LOTO) isolation point safety metrics.
/// </summary>
public record LotoComplianceMetrics(
    int TotalIsolationPoints,
    int LockedPointsCount,
    int VerifiedZeroEnergyPointsCount,
    int OpenPointsCount,
    int ActiveLotoPermitsCount,
    int TotalActiveLocks,
    int OrphanedLocksCount,
    double LotoComplianceRate
);

/// <summary>
/// Atmospheric gas telemetry alert metrics for a specific plant zone.
/// </summary>
public record ZoneGasAlertMetrics(
    string ZoneCode,
    int TotalReadings,
    int ActionAlarmsCount,
    int EvacuationAlarmsCount,
    int TotalAlerts,
    double AlertRatePerHundredReadings,
    string HighestRiskGas
);

/// <summary>
/// Executive and operational analytics summary aggregating equipment inventory,
/// calibration compliance, utilization rates, LOTO metrics, and gas alert frequencies.
/// </summary>
public record EquipmentAnalyticsSummary(
    int TotalEquipment,
    int ActiveEquipmentInService,
    Dictionary<EquipmentType, int> TotalEquipmentByType,
    IReadOnlyList<EquipmentTypeAnalytics> EquipmentTypeBreakdown,
    CalibrationComplianceMetrics CalibrationMetrics,
    double CalibrationComplianceRate,
    int OverdueCalibrationsCount,
    double AverageEquipmentUtilizationPercentage,
    double AverageOperatingHoursPerAsset,
    LotoComplianceMetrics LotoMetrics,
    Dictionary<string, int> GasAlertFrequencyByZone,
    IReadOnlyList<ZoneGasAlertMetrics> ZoneGasAlertBreakdown,
    DateTime GeneratedAtUtc
);

#endregion

#region Service Contract

/// <summary>
/// Service interface aggregating safety equipment analytics in ClearToWork AI.
/// Compiles fleet inventory distributions, calibration compliance rates, overdue equipment counts,
/// field utilization rates, LOTO energy isolation metrics, and zone-based gas alert frequencies.
/// </summary>
public interface IEquipmentAnalyticsService
{
    /// <summary>
    /// Aggregates all safety equipment metrics into a consolidated <see cref="EquipmentAnalyticsSummary"/> DTO.
    /// </summary>
    /// <param name="equipment">Collection of all equipment assets.</param>
    /// <param name="checkouts">Historical and active checkout records.</param>
    /// <param name="isolationPoints">Facility energy isolation points.</param>
    /// <param name="gasReadings">Atmospheric gas monitor readings.</param>
    /// <param name="workerPresences">Optional real-time worker site presence records.</param>
    /// <param name="asOfUtc">Evaluation timestamp; defaults to UTC now.</param>
    /// <returns>A comprehensive <see cref="EquipmentAnalyticsSummary"/> DTO.</returns>
    EquipmentAnalyticsSummary AggregateEquipmentAnalytics(
        IEnumerable<EquipmentItem> equipment,
        IEnumerable<EquipmentCheckoutRecord> checkouts,
        IEnumerable<IsolationPoint> isolationPoints,
        IEnumerable<GasReading> gasReadings,
        IEnumerable<WorkerSitePresence>? workerPresences = null,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Calculates calibration compliance rate and identifies overdue assets across the equipment registry.
    /// </summary>
    CalibrationComplianceMetrics CalculateCalibrationCompliance(
        IEnumerable<EquipmentItem> equipment,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Computes fleet utilization statistics based on active equipment checkouts and accrued operating hours.
    /// </summary>
    EquipmentUtilizationMetrics CalculateUtilization(
        IEnumerable<EquipmentItem> equipment,
        IEnumerable<EquipmentCheckoutRecord> checkouts,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Evaluates Lockout/Tagout (LOTO) isolation point compliance, lock application, and orphaned padlocks.
    /// </summary>
    LotoComplianceMetrics CalculateLotoMetrics(
        IEnumerable<IsolationPoint> isolationPoints,
        IEnumerable<WorkerSitePresence>? workerPresences = null,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Aggregates gas monitor alert counts (Action and Evacuation thresholds) categorized by plant zone.
    /// </summary>
    Dictionary<string, int> CalculateGasAlertFrequencyByZone(
        IEnumerable<GasReading> gasReadings);

    /// <summary>
    /// Generates detailed zone-by-zone gas telemetry alert breakdowns with risk classifications.
    /// </summary>
    IReadOnlyList<ZoneGasAlertMetrics> CalculateZoneGasAlertBreakdown(
        IEnumerable<GasReading> gasReadings);
}

#endregion

#region Implementation

/// <summary>
/// Production implementation of <see cref="IEquipmentAnalyticsService"/> aggregating equipment health,
/// calibration compliance, utilization velocity, LOTO safety metrics, and atmospheric gas hazard distributions.
/// </summary>
public class EquipmentAnalyticsService : IEquipmentAnalyticsService
{
    private readonly IGasReadingAnalyzer _gasAnalyzer;
    private readonly ILOTOComplianceChecker _lotoChecker;

    /// <summary>
    /// Initializes a new instance of <see cref="EquipmentAnalyticsService"/>.
    /// </summary>
    /// <param name="gasAnalyzer">Optional gas reading analyzer; uses default if omitted.</param>
    /// <param name="lotoChecker">Optional LOTO compliance checker; uses default if omitted.</param>
    public EquipmentAnalyticsService(
        IGasReadingAnalyzer? gasAnalyzer = null,
        ILOTOComplianceChecker? lotoChecker = null)
    {
        _gasAnalyzer = gasAnalyzer ?? new GasReadingAnalyzer();
        _lotoChecker = lotoChecker ?? new LOTOComplianceChecker();
    }

    /// <inheritdoc />
    public EquipmentAnalyticsSummary AggregateEquipmentAnalytics(
        IEnumerable<EquipmentItem> equipment,
        IEnumerable<EquipmentCheckoutRecord> checkouts,
        IEnumerable<IsolationPoint> isolationPoints,
        IEnumerable<GasReading> gasReadings,
        IEnumerable<WorkerSitePresence>? workerPresences = null,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(equipment);
        ArgumentNullException.ThrowIfNull(checkouts);
        ArgumentNullException.ThrowIfNull(isolationPoints);
        ArgumentNullException.ThrowIfNull(gasReadings);

        var now = asOfUtc ?? DateTime.UtcNow;
        var equipmentList = equipment.ToList();
        var totalEquipment = equipmentList.Count;

        var activeEquipmentList = equipmentList
            .Where(e => e.Status != EquipmentStatus.Decommissioned)
            .ToList();
        var activeInService = activeEquipmentList.Count;

        // 1. Total equipment by type breakdown
        var totalEquipmentByType = new Dictionary<EquipmentType, int>();
        foreach (var type in Enum.GetValues<EquipmentType>())
        {
            totalEquipmentByType[type] = 0;
        }

        foreach (var item in equipmentList)
        {
            totalEquipmentByType[item.Type]++;
        }

        var typeBreakdown = Enum.GetValues<EquipmentType>()
            .Select(type =>
            {
                var matching = equipmentList.Where(e => e.Type == type).ToList();
                var count = matching.Count;
                var avail = matching.Count(e => e.Status == EquipmentStatus.Available);
                var chk = matching.Count(e => e.Status == EquipmentStatus.CheckedOut);
                var maint = matching.Count(e => e.Status == EquipmentStatus.InMaintenance);
                var quar = matching.Count(e => e.Status == EquipmentStatus.Quarantined);
                var rate = count > 0 ? Math.Round(((double)chk / count) * 100.0, 1) : 0.0;

                return new EquipmentTypeAnalytics(
                    Type: type,
                    TotalCount: count,
                    AvailableCount: avail,
                    CheckedOutCount: chk,
                    InMaintenanceCount: maint,
                    QuarantinedCount: quar,
                    UtilizationRate: rate
                );
            })
            .ToList();

        // 2 & 3. Calibration Compliance Rate & Overdue Count
        var calibrationMetrics = CalculateCalibrationCompliance(equipmentList, now);

        // 4. Average Equipment Utilization
        var utilizationMetrics = CalculateUtilization(equipmentList, checkouts, now);

        // 5. LOTO Compliance Metrics
        var lotoMetrics = CalculateLotoMetrics(isolationPoints, workerPresences, now);

        // 6. Gas Alert Frequency by Zone
        var gasAlertFrequency = CalculateGasAlertFrequencyByZone(gasReadings);
        var zoneBreakdown = CalculateZoneGasAlertBreakdown(gasReadings);

        return new EquipmentAnalyticsSummary(
            TotalEquipment: totalEquipment,
            ActiveEquipmentInService: activeInService,
            TotalEquipmentByType: totalEquipmentByType,
            EquipmentTypeBreakdown: typeBreakdown,
            CalibrationMetrics: calibrationMetrics,
            CalibrationComplianceRate: calibrationMetrics.ComplianceRate,
            OverdueCalibrationsCount: calibrationMetrics.OverdueCount,
            AverageEquipmentUtilizationPercentage: utilizationMetrics.FleetCheckoutUtilizationPercentage,
            AverageOperatingHoursPerAsset: utilizationMetrics.AverageOperatingHoursPerAsset,
            LotoMetrics: lotoMetrics,
            GasAlertFrequencyByZone: gasAlertFrequency,
            ZoneGasAlertBreakdown: zoneBreakdown,
            GeneratedAtUtc: now
        );
    }

    /// <inheritdoc />
    public CalibrationComplianceMetrics CalculateCalibrationCompliance(
        IEnumerable<EquipmentItem> equipment,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(equipment);

        var now = asOfUtc ?? DateTime.UtcNow;
        var requiringCalibration = equipment
            .Where(e => e.RequiresCalibration && e.Status != EquipmentStatus.Decommissioned)
            .ToList();

        var total = requiringCalibration.Count;
        if (total == 0)
        {
            return new CalibrationComplianceMetrics(
                TotalRequiringCalibration: 0,
                CalibratedAndValidCount: 0,
                ExpiringSoonCount: 0,
                OverdueCount: 0,
                ComplianceRate: 100.0
            );
        }

        var validCount = 0;
        var expiringSoonCount = 0;
        var overdueCount = 0;
        const int warningWindowDays = 7;

        foreach (var item in requiringCalibration)
        {
            if (item.Status == EquipmentStatus.Quarantined)
            {
                overdueCount++;
                continue;
            }

            if (!item.NextCalibrationDueDate.HasValue || item.NextCalibrationDueDate.Value < now)
            {
                overdueCount++;
            }
            else
            {
                validCount++;
                var daysRemaining = (item.NextCalibrationDueDate.Value - now).TotalDays;
                if (daysRemaining <= warningWindowDays)
                {
                    expiringSoonCount++;
                }
            }
        }

        var complianceRate = Math.Round(((double)validCount / total) * 100.0, 1);

        return new CalibrationComplianceMetrics(
            TotalRequiringCalibration: total,
            CalibratedAndValidCount: validCount,
            ExpiringSoonCount: expiringSoonCount,
            OverdueCount: overdueCount,
            ComplianceRate: complianceRate
        );
    }

    /// <inheritdoc />
    public EquipmentUtilizationMetrics CalculateUtilization(
        IEnumerable<EquipmentItem> equipment,
        IEnumerable<EquipmentCheckoutRecord> checkouts,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(equipment);
        ArgumentNullException.ThrowIfNull(checkouts);

        var activeEquipment = equipment
            .Where(e => e.Status != EquipmentStatus.Decommissioned)
            .ToList();

        var totalActive = activeEquipment.Count;
        if (totalActive == 0)
        {
            return new EquipmentUtilizationMetrics(0, 0, 0.0, 0.0, 0.0);
        }

        var checkedOutCount = activeEquipment.Count(e => e.Status == EquipmentStatus.CheckedOut);
        var checkoutRate = Math.Round(((double)checkedOutCount / totalActive) * 100.0, 1);

        var totalHours = activeEquipment.Sum(e => e.OperatingHours);
        var avgHours = Math.Round(totalHours / totalActive, 1);

        return new EquipmentUtilizationMetrics(
            TotalActiveEquipment: totalActive,
            CheckedOutCount: checkedOutCount,
            FleetCheckoutUtilizationPercentage: checkoutRate,
            AverageOperatingHoursPerAsset: avgHours,
            TotalOperatingHoursLogged: Math.Round(totalHours, 1)
        );
    }

    /// <inheritdoc />
    public LotoComplianceMetrics CalculateLotoMetrics(
        IEnumerable<IsolationPoint> isolationPoints,
        IEnumerable<WorkerSitePresence>? workerPresences = null,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(isolationPoints);

        var now = asOfUtc ?? DateTime.UtcNow;
        var pointsList = isolationPoints.ToList();
        var totalPoints = pointsList.Count;

        if (totalPoints == 0)
        {
            return new LotoComplianceMetrics(0, 0, 0, 0, 0, 0, 0, 100.0);
        }

        var lockedCount = pointsList.Count(p => p.State is IsolationState.LockedIsolated or IsolationState.VerifiedZeroEnergy);
        var zeroEnergyCount = pointsList.Count(p => p.ZeroEnergyVerified);
        var openCount = pointsList.Count(p => p.State == IsolationState.OpenDeIsolated);

        var activePermitIds = pointsList
            .Where(p => p.ActivePermitId.HasValue)
            .Select(p => p.ActivePermitId!.Value)
            .Distinct()
            .Count();

        var totalLocks = pointsList.Sum(p => p.AppliedLocks.Count);

        var presences = workerPresences?.ToList() ?? new List<WorkerSitePresence>();
        var orphanedLocks = _lotoChecker.DetectOrphanedLocks(pointsList, presences, now);
        var orphanedCount = orphanedLocks.Count;

        // Compliance rate: percentage of locked points that are properly zero-energy verified without orphaned locks
        double complianceRate;
        if (lockedCount > 0)
        {
            var fullyCompliantLocked = pointsList.Count(p =>
                (p.State is IsolationState.LockedIsolated or IsolationState.VerifiedZeroEnergy) &&
                p.ZeroEnergyVerified &&
                p.AppliedLocks.Count > 0 &&
                !orphanedLocks.Any(o => o.IsolationPointId == p.Id));

            complianceRate = Math.Round(((double)fullyCompliantLocked / lockedCount) * 100.0, 1);
        }
        else
        {
            // All points open and safe
            complianceRate = 100.0;
        }

        return new LotoComplianceMetrics(
            TotalIsolationPoints: totalPoints,
            LockedPointsCount: lockedCount,
            VerifiedZeroEnergyPointsCount: zeroEnergyCount,
            OpenPointsCount: openCount,
            ActiveLotoPermitsCount: activePermitIds,
            TotalActiveLocks: totalLocks,
            OrphanedLocksCount: orphanedCount,
            LotoComplianceRate: complianceRate
        );
    }

    /// <inheritdoc />
    public Dictionary<string, int> CalculateGasAlertFrequencyByZone(
        IEnumerable<GasReading> gasReadings)
    {
        ArgumentNullException.ThrowIfNull(gasReadings);

        var frequency = new Dictionary<string, int>();

        foreach (var reading in gasReadings)
        {
            var zone = string.IsNullOrWhiteSpace(reading.ZoneCode) ? "UNKNOWN_ZONE" : reading.ZoneCode.Trim().ToUpperInvariant();
            if (!frequency.ContainsKey(zone))
            {
                frequency[zone] = 0;
            }

            var alarmLevel = _gasAnalyzer.DetermineAlarmLevel(reading);
            if (alarmLevel is GasAlarmLevel.Action or GasAlarmLevel.Evacuation)
            {
                frequency[zone]++;
            }
        }

        return frequency;
    }

    /// <inheritdoc />
    public IReadOnlyList<ZoneGasAlertMetrics> CalculateZoneGasAlertBreakdown(
        IEnumerable<GasReading> gasReadings)
    {
        ArgumentNullException.ThrowIfNull(gasReadings);

        var list = gasReadings.ToList();
        var groups = list.GroupBy(r => string.IsNullOrWhiteSpace(r.ZoneCode) ? "UNKNOWN_ZONE" : r.ZoneCode.Trim().ToUpperInvariant());
        var results = new List<ZoneGasAlertMetrics>();

        foreach (var group in groups)
        {
            var zone = group.Key;
            var totalReadings = group.Count();
            var actionCount = 0;
            var evacuationCount = 0;

            var h2sViolations = 0;
            var lelViolations = 0;
            var o2Violations = 0;
            var coViolations = 0;

            foreach (var r in group)
            {
                var eval = _gasAnalyzer.EvaluateReading(r);
                if (eval.HighestAlarmLevel == GasAlarmLevel.Evacuation)
                {
                    evacuationCount++;
                }
                else if (eval.HighestAlarmLevel == GasAlarmLevel.Action)
                {
                    actionCount++;
                }

                if (r.H2sPpm >= 5.0) h2sViolations++;
                if (r.LelPercentage >= 5.0) lelViolations++;
                if (r.OxygenPercentage < 19.5 || r.OxygenPercentage > 23.5) o2Violations++;
                if (r.CoPpm >= 25.0) coViolations++;
            }

            var totalAlerts = actionCount + evacuationCount;
            var alertRate = totalReadings > 0
                ? Math.Round(((double)totalAlerts / totalReadings) * 100.0, 1)
                : 0.0;

            // Determine highest risk gas in this zone
            var gasViolationCounts = new Dictionary<string, int>
            {
                ["H2S (Toxic)"] = h2sViolations,
                ["LEL (Combustible)"] = lelViolations,
                ["O2 (Asphyxiation/Enrichment)"] = o2Violations,
                ["CO (Carbon Monoxide)"] = coViolations
            };

            var highestRiskGas = gasViolationCounts.OrderByDescending(kv => kv.Value).First();
            var riskLabel = highestRiskGas.Value > 0 ? highestRiskGas.Key : "None (All Safe)";

            results.Add(new ZoneGasAlertMetrics(
                ZoneCode: zone,
                TotalReadings: totalReadings,
                ActionAlarmsCount: actionCount,
                EvacuationAlarmsCount: evacuationCount,
                TotalAlerts: totalAlerts,
                AlertRatePerHundredReadings: alertRate,
                HighestRiskGas: riskLabel
            ));
        }

        return results.OrderByDescending(z => z.TotalAlerts).ToList();
    }
}

#endregion
