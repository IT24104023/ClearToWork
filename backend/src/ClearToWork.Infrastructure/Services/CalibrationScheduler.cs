using System.Text.Json.Serialization;

namespace ClearToWork.Infrastructure.Services;

#region Domain Enums & Models

/// <summary>
/// Types of atmospheric gas monitoring instruments deployed across oil &amp; gas permit zones.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GasMonitorType
{
    /// <summary>
    /// Single-gas Hydrogen Sulfide detector (deadly sour gas, toxic in trace ppm). Requires daily bump test and calibration verification.
    /// </summary>
    H2S_SingleGas = 1,

    /// <summary>
    /// Combustible Lower Explosive Limit hydrocarbon gas detector (catalytic bead or NDIR). Requires weekly calibration.
    /// </summary>
    LEL_Combustible = 2,

    /// <summary>
    /// Standard 4-gas confined space monitor (O2, LEL, H2S, CO). Requires monthly bench calibration.
    /// </summary>
    MultiGas_4Gas = 3,

    /// <summary>
    /// Photoionization Detector for Volatile Organic Compounds (VOCs, benzene, aromatics).
    /// </summary>
    PID_VOC = 4,

    /// <summary>
    /// Dedicated electrochemical oxygen depletion/enrichment sensor.
    /// </summary>
    Oxygen_SingleGas = 5,

    /// <summary>
    /// Single-gas toxic Carbon Monoxide detector.
    /// </summary>
    CarbonMonoxide_SingleGas = 6
}

/// <summary>
/// Urgency classification for gas detector calibration alerts and notifications.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReminderUrgency
{
    /// <summary>
    /// Calibration is in order; regular reminder within normal operational limits.
    /// </summary>
    Informational = 1,

    /// <summary>
    /// Calibration expires within the advance warning window (e.g., 1-3 days).
    /// </summary>
    DueSoon = 2,

    /// <summary>
    /// Calibration has expired; device must not be issued for permit work.
    /// </summary>
    Overdue = 3,

    /// <summary>
    /// Device has critically exceeded calibration or operating hours limit, or is actively assigned to an active permit.
    /// </summary>
    Critical = 4
}

/// <summary>
/// Standardized interval configuration and operational constraints for a specific gas monitor type.
/// </summary>
public record CalibrationIntervalRule(
    GasMonitorType DeviceType,
    TimeSpan MaxCalendarInterval,
    double MaxOperatingHours,
    bool RequiresDailyBumpTest,
    int AdvanceWarningDays,
    string Description
);

/// <summary>
/// Operational entity representing a field-deployed gas detector unit.
/// </summary>
public class GasMonitorDevice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SerialNumber { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public GasMonitorType DeviceType { get; set; }
    public DateTime LastCalibrationDate { get; set; }
    public double CumulativeOperatingHoursSinceCalibration { get; set; }
    public bool IsActive { get; set; } = true;
    public string? AssignedZone { get; set; }
    public string? AssignedPermitNumber { get; set; }
    public string? CustodianWorkerBadge { get; set; }
}

/// <summary>
/// Result of an individual calibration scheduling assessment.
/// </summary>
public record CalibrationScheduleResult(
    Guid DeviceId,
    string SerialNumber,
    GasMonitorType DeviceType,
    DateTime LastCalibrationDate,
    DateTime NextCalibrationDueDate,
    double OperatingHoursRemaining,
    bool IsOverdue,
    int DaysUntilDue,
    string LimitingFactor,
    string Recommendation
);

/// <summary>
/// Calibration notification reminder emitted for safety officers and instrument technicians.
/// </summary>
public record CalibrationReminder(
    Guid DeviceId,
    string SerialNumber,
    GasMonitorType DeviceType,
    DateTime DueDate,
    int DaysRemaining,
    double OperatingHoursLogged,
    ReminderUrgency Urgency,
    string Message,
    DateTime GeneratedAtUtc
);

/// <summary>
/// Fleet-wide calibration compliance summary for HSE auditing and readiness metrics.
/// </summary>
public record FleetCalibrationSummary(
    int TotalMonitors,
    int CompliantCount,
    int DueSoonCount,
    int OverdueCount,
    double CompliancePercentage,
    IReadOnlyDictionary<GasMonitorType, int> OverdueByType,
    IReadOnlyList<CalibrationScheduleResult> OverdueDetails,
    DateTime EvaluatedAtUtc
);

#endregion

#region Service Contract

/// <summary>
/// Service interface governing gas monitor calibration scheduling, interval policy enforcement,
/// overdue asset detection, and preventative calibration reminders in ClearToWork AI.
/// </summary>
public interface ICalibrationScheduler
{
    /// <summary>
    /// Retrieves the safety calibration interval rule governing the specified detector type.
    /// </summary>
    CalibrationIntervalRule GetRuleForDeviceType(GasMonitorType deviceType);

    /// <summary>
    /// Computes the exact next calibration due date considering both calendar interval rules
    /// and accrued sensor operating hours.
    /// </summary>
    DateTime CalculateNextCalibrationDueDate(
        GasMonitorType deviceType,
        DateTime lastCalibration,
        double currentOperatingHours,
        double estimatedDailyUsageHours = 8.0);

    /// <summary>
    /// Evaluates a single gas monitoring device and schedules its next required calibration.
    /// </summary>
    CalibrationScheduleResult ScheduleNextCalibration(
        GasMonitorDevice device,
        double estimatedDailyUsageHours = 8.0,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Filters and returns all monitors that are currently past their calibration due date
    /// or exceeded maximum allowable operating hours.
    /// </summary>
    IReadOnlyList<GasMonitorDevice> IdentifyOverdueDevices(
        IEnumerable<GasMonitorDevice> devices,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Generates structured calibration reminders for safety technicians, prioritizing critical and impending expirations.
    /// </summary>
    IReadOnlyList<CalibrationReminder> GenerateReminders(
        IEnumerable<GasMonitorDevice> devices,
        int? overrideWarningDays = null,
        DateTime? asOfUtc = null);

    /// <summary>
    /// Checks whether an individual device requires immediate calibration before field deployment.
    /// </summary>
    bool IsCalibrationOverdue(GasMonitorDevice device, DateTime? asOfUtc = null);

    /// <summary>
    /// Produces a fleet-wide compliance audit summary across all active gas monitoring equipment.
    /// </summary>
    FleetCalibrationSummary EvaluateFleetCompliance(
        IEnumerable<GasMonitorDevice> devices,
        DateTime? asOfUtc = null);
}

#endregion

#region Implementation

/// <summary>
/// Production implementation of <see cref="ICalibrationScheduler"/> enforcing OSHA 1910.146,
/// OSHA 1910.1200, and IOGP Life-Saving Rules for atmospheric gas monitoring calibration.
/// Supports strict calibration interval rules:
/// - Daily (24h / 12 op-hours) for H2S toxic monitors.
/// - Weekly (7 days / 50 op-hours) for LEL combustible monitors.
/// - Monthly (30 days / 200 op-hours) for multi-gas (4-gas) instruments.
/// - Bi-weekly (14 days / 100 op-hours) for PID (VOC) and toxic single-gas monitors.
/// </summary>
public class CalibrationScheduler : ICalibrationScheduler
{
    private static readonly Dictionary<GasMonitorType, CalibrationIntervalRule> StandardRules = new()
    {
        [GasMonitorType.H2S_SingleGas] = new CalibrationIntervalRule(
            DeviceType: GasMonitorType.H2S_SingleGas,
            MaxCalendarInterval: TimeSpan.FromDays(1),
            MaxOperatingHours: 12.0,
            RequiresDailyBumpTest: true,
            AdvanceWarningDays: 1,
            Description: "H2S monitors require daily bump testing and calibration verification due to rapid sensor poisoning risks."
        ),
        [GasMonitorType.LEL_Combustible] = new CalibrationIntervalRule(
            DeviceType: GasMonitorType.LEL_Combustible,
            MaxCalendarInterval: TimeSpan.FromDays(7),
            MaxOperatingHours: 50.0,
            RequiresDailyBumpTest: true,
            AdvanceWarningDays: 2,
            Description: "Combustible catalytic bead/NDIR LEL sensors require weekly full-span calibration."
        ),
        [GasMonitorType.MultiGas_4Gas] = new CalibrationIntervalRule(
            DeviceType: GasMonitorType.MultiGas_4Gas,
            MaxCalendarInterval: TimeSpan.FromDays(30),
            MaxOperatingHours: 200.0,
            RequiresDailyBumpTest: true,
            AdvanceWarningDays: 5,
            Description: "Standard 4-gas confined space instruments require monthly 4-mix gas cylinder calibration."
        ),
        [GasMonitorType.PID_VOC] = new CalibrationIntervalRule(
            DeviceType: GasMonitorType.PID_VOC,
            MaxCalendarInterval: TimeSpan.FromDays(14),
            MaxOperatingHours: 100.0,
            RequiresDailyBumpTest: true,
            AdvanceWarningDays: 3,
            Description: "Photoionization detectors (10.6 eV lamp) require bi-weekly calibration using Isobutylene span gas."
        ),
        [GasMonitorType.Oxygen_SingleGas] = new CalibrationIntervalRule(
            DeviceType: GasMonitorType.Oxygen_SingleGas,
            MaxCalendarInterval: TimeSpan.FromDays(30),
            MaxOperatingHours: 200.0,
            RequiresDailyBumpTest: false,
            AdvanceWarningDays: 5,
            Description: "Electrochemical O2 sensors require monthly 20.9% fresh air baseline and zero-grade nitrogen calibration."
        ),
        [GasMonitorType.CarbonMonoxide_SingleGas] = new CalibrationIntervalRule(
            DeviceType: GasMonitorType.CarbonMonoxide_SingleGas,
            MaxCalendarInterval: TimeSpan.FromDays(14),
            MaxOperatingHours: 100.0,
            RequiresDailyBumpTest: true,
            AdvanceWarningDays: 3,
            Description: "CO single-gas sensors require bi-weekly span calibration using certified 50 ppm CO."
        )
    };

    /// <inheritdoc />
    public CalibrationIntervalRule GetRuleForDeviceType(GasMonitorType deviceType)
    {
        if (StandardRules.TryGetValue(deviceType, out var rule))
        {
            return rule;
        }

        // Fallback default rule for unspecified instruments: weekly / 50 hours
        return new CalibrationIntervalRule(
            DeviceType: deviceType,
            MaxCalendarInterval: TimeSpan.FromDays(7),
            MaxOperatingHours: 50.0,
            RequiresDailyBumpTest: true,
            AdvanceWarningDays: 2,
            Description: "Default protective calibration interval: 7 days or 50 operational hours."
        );
    }

    /// <inheritdoc />
    public DateTime CalculateNextCalibrationDueDate(
        GasMonitorType deviceType,
        DateTime lastCalibration,
        double currentOperatingHours,
        double estimatedDailyUsageHours = 8.0)
    {
        var rule = GetRuleForDeviceType(deviceType);

        // 1. Calculate calendar limit
        var calendarDueDate = lastCalibration.Add(rule.MaxCalendarInterval);

        // 2. Calculate operational hours limit
        var hoursRemaining = Math.Max(0.0, rule.MaxOperatingHours - currentOperatingHours);

        if (estimatedDailyUsageHours <= 0)
        {
            estimatedDailyUsageHours = 8.0; // Standard shift baseline
        }

        var daysRemainingByUsage = hoursRemaining / estimatedDailyUsageHours;
        var hoursDueDate = lastCalibration.AddDays(daysRemainingByUsage);

        // The binding due date is whichever occurs first (most conservative safety limit)
        return calendarDueDate <= hoursDueDate ? calendarDueDate : hoursDueDate;
    }

    /// <inheritdoc />
    public CalibrationScheduleResult ScheduleNextCalibration(
        GasMonitorDevice device,
        double estimatedDailyUsageHours = 8.0,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(device);

        var now = asOfUtc ?? DateTime.UtcNow;
        var rule = GetRuleForDeviceType(device.DeviceType);

        var calendarDueDate = device.LastCalibrationDate.Add(rule.MaxCalendarInterval);
        var hoursRemaining = rule.MaxOperatingHours - device.CumulativeOperatingHoursSinceCalibration;

        var dailyUsage = estimatedDailyUsageHours > 0 ? estimatedDailyUsageHours : 8.0;
        var daysRemainingByUsage = Math.Max(0.0, hoursRemaining) / dailyUsage;
        var hoursDueDate = device.LastCalibrationDate.AddDays(daysRemainingByUsage);

        DateTime nextDueDate;
        string limitingFactor;

        if (hoursRemaining <= 0)
        {
            // Operating hours already completely exhausted
            nextDueDate = device.LastCalibrationDate;
            limitingFactor = "OperatingHoursExhausted";
        }
        else if (calendarDueDate <= hoursDueDate)
        {
            nextDueDate = calendarDueDate;
            limitingFactor = "CalendarInterval";
        }
        else
        {
            nextDueDate = hoursDueDate;
            limitingFactor = "OperatingHoursUsageRate";
        }

        var isOverdue = now > nextDueDate || device.CumulativeOperatingHoursSinceCalibration >= rule.MaxOperatingHours;
        var daysUntilDue = (int)Math.Ceiling((nextDueDate - now).TotalDays);

        string recommendation;
        if (isOverdue)
        {
            recommendation = $"LOCKOUT: {device.SerialNumber} ({device.DeviceType}) calibration expired. Immediate recalibration required before permit issuance.";
        }
        else if (daysUntilDue <= rule.AdvanceWarningDays)
        {
            recommendation = $"WARNING: Calibration due in {daysUntilDue} day(s). Schedule sensor bench calibration with certified gas cylinder.";
        }
        else
        {
            recommendation = $"COMPLIANT: Calibration valid until {nextDueDate:yyyy-MM-dd HH:mm} UTC ({hoursRemaining:F1} operating hours remaining).";
        }

        return new CalibrationScheduleResult(
            DeviceId: device.Id,
            SerialNumber: device.SerialNumber,
            DeviceType: device.DeviceType,
            LastCalibrationDate: device.LastCalibrationDate,
            NextCalibrationDueDate: nextDueDate,
            OperatingHoursRemaining: Math.Max(0.0, hoursRemaining),
            IsOverdue: isOverdue,
            DaysUntilDue: daysUntilDue,
            LimitingFactor: limitingFactor,
            Recommendation: recommendation
        );
    }

    /// <inheritdoc />
    public IReadOnlyList<GasMonitorDevice> IdentifyOverdueDevices(
        IEnumerable<GasMonitorDevice> devices,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(devices);

        var now = asOfUtc ?? DateTime.UtcNow;
        var overdueList = new List<GasMonitorDevice>();

        foreach (var device in devices)
        {
            if (!device.IsActive)
            {
                continue;
            }

            if (IsCalibrationOverdue(device, now))
            {
                overdueList.Add(device);
            }
        }

        return overdueList;
    }

    /// <inheritdoc />
    public IReadOnlyList<CalibrationReminder> GenerateReminders(
        IEnumerable<GasMonitorDevice> devices,
        int? overrideWarningDays = null,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(devices);

        var now = asOfUtc ?? DateTime.UtcNow;
        var reminders = new List<CalibrationReminder>();

        foreach (var device in devices)
        {
            if (!device.IsActive)
            {
                continue;
            }

            var rule = GetRuleForDeviceType(device.DeviceType);
            var schedule = ScheduleNextCalibration(device, asOfUtc: now);
            var warningWindow = overrideWarningDays ?? rule.AdvanceWarningDays;

            ReminderUrgency urgency;
            string message;

            if (schedule.IsOverdue)
            {
                // If the overdue device is currently assigned to an active permit, escalate to Critical
                if (!string.IsNullOrWhiteSpace(device.AssignedPermitNumber))
                {
                    urgency = ReminderUrgency.Critical;
                    message = $"CRITICAL SAFETY ALERT: Gas monitor {device.SerialNumber} ({device.DeviceType}) is active on Permit {device.AssignedPermitNumber} but calibration is OVERDUE by {Math.Abs(schedule.DaysUntilDue)} day(s). Suspend permit and swap unit immediately.";
                }
                else
                {
                    urgency = ReminderUrgency.Overdue;
                    message = $"OVERDUE: Gas monitor {device.SerialNumber} ({device.DeviceType}) calibration expired on {schedule.NextCalibrationDueDate:yyyy-MM-dd}. Unit must be quarantined.";
                }
            }
            else if (schedule.DaysUntilDue <= warningWindow)
            {
                urgency = ReminderUrgency.DueSoon;
                message = $"DUE SOON: Gas monitor {device.SerialNumber} calibration due in {schedule.DaysUntilDue} day(s) ({schedule.NextCalibrationDueDate:yyyy-MM-dd}). Arrange recalibration.";
            }
            else
            {
                urgency = ReminderUrgency.Informational;
                message = $"OK: Gas monitor {device.SerialNumber} calibrated. Next due {schedule.NextCalibrationDueDate:yyyy-MM-dd}.";
            }

            reminders.Add(new CalibrationReminder(
                DeviceId: device.Id,
                SerialNumber: device.SerialNumber,
                DeviceType: device.DeviceType,
                DueDate: schedule.NextCalibrationDueDate,
                DaysRemaining: schedule.DaysUntilDue,
                OperatingHoursLogged: device.CumulativeOperatingHoursSinceCalibration,
                Urgency: urgency,
                Message: message,
                GeneratedAtUtc: now
            ));
        }

        // Return ordered by severity: Critical first, then Overdue, DueSoon, Informational
        return reminders
            .OrderByDescending(r => r.Urgency)
            .ThenBy(r => r.DaysRemaining)
            .ToList();
    }

    /// <inheritdoc />
    public bool IsCalibrationOverdue(GasMonitorDevice device, DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(device);

        var now = asOfUtc ?? DateTime.UtcNow;
        var rule = GetRuleForDeviceType(device.DeviceType);

        // Check 1: Calendar interval elapsed
        if (now > device.LastCalibrationDate.Add(rule.MaxCalendarInterval))
        {
            return true;
        }

        // Check 2: Exceeded allowable operating hours on sensor
        if (device.CumulativeOperatingHoursSinceCalibration >= rule.MaxOperatingHours)
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public FleetCalibrationSummary EvaluateFleetCompliance(
        IEnumerable<GasMonitorDevice> devices,
        DateTime? asOfUtc = null)
    {
        ArgumentNullException.ThrowIfNull(devices);

        var now = asOfUtc ?? DateTime.UtcNow;
        var deviceList = devices.Where(d => d.IsActive).ToList();

        var total = deviceList.Count;
        var compliantCount = 0;
        var dueSoonCount = 0;
        var overdueCount = 0;
        var overdueByType = new Dictionary<GasMonitorType, int>();
        var overdueDetails = new List<CalibrationScheduleResult>();

        foreach (var type in Enum.GetValues<GasMonitorType>())
        {
            overdueByType[type] = 0;
        }

        foreach (var device in deviceList)
        {
            var schedule = ScheduleNextCalibration(device, asOfUtc: now);
            var rule = GetRuleForDeviceType(device.DeviceType);

            if (schedule.IsOverdue)
            {
                overdueCount++;
                overdueByType[device.DeviceType]++;
                overdueDetails.Add(schedule);
            }
            else if (schedule.DaysUntilDue <= rule.AdvanceWarningDays)
            {
                dueSoonCount++;
            }
            else
            {
                compliantCount++;
            }
        }

        var complianceRate = total > 0
            ? Math.Round(((double)(total - overdueCount) / total) * 100.0, 1)
            : 100.0;

        return new FleetCalibrationSummary(
            TotalMonitors: total,
            CompliantCount: compliantCount,
            DueSoonCount: dueSoonCount,
            OverdueCount: overdueCount,
            CompliancePercentage: complianceRate,
            OverdueByType: overdueByType,
            OverdueDetails: overdueDetails,
            EvaluatedAtUtc: now
        );
    }
}

#endregion
