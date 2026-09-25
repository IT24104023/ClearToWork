using System.Text.Json.Serialization;

namespace ClearToWork.Infrastructure.Services;

#region Domain Enums & Models

/// <summary>
/// Target atmospheric gases monitored in hot-work, confined-space, and hazardous process zones.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GasType
{
    /// <summary>
    /// Atmospheric Oxygen (O2). Baseline 20.9%. OSHA safe range: 19.5% to 23.5%.
    /// </summary>
    Oxygen = 1,

    /// <summary>
    /// Combustible gases and hydrocarbons measured in Lower Explosive Limit percentage (% LEL). Safe threshold: &lt; 10% LEL.
    /// </summary>
    Combustible_LEL = 2,

    /// <summary>
    /// Hydrogen Sulfide (H2S), toxic sour gas measured in parts per million (ppm). Safe threshold: &lt; 10 ppm.
    /// </summary>
    HydrogenSulfide_H2S = 3,

    /// <summary>
    /// Carbon Monoxide (CO), toxic byproduct of incomplete combustion measured in ppm. Safe threshold: &lt; 25 ppm.
    /// </summary>
    CarbonMonoxide_CO = 4,

    /// <summary>
    /// Volatile Organic Compounds (VOCs) measured via PID sensor in ppm.
    /// </summary>
    VolatileOrganic_VOC = 5
}

/// <summary>
/// Operational atmospheric alarm escalation levels in accordance with OSHA 1910.146 and IOGP standards.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GasAlarmLevel
{
    /// <summary>
    /// Atmospheric readings are entirely within normal, safe breathable ranges.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Atmospheric readings exhibit minor fluctuations or approach pre-alarm margins.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Action Level reached (e.g., LEL &gt;= 5%, H2S &gt;= 5 ppm, O2 &lt; 20.0% or &gt; 22.5%, CO &gt;= 25 ppm, or rapid rising trend).
    /// Hot work is suspended, ventilation increased, and investigation initiated.
    /// </summary>
    Action = 2,

    /// <summary>
    /// Evacuation Level breached (O2 &lt; 19.5% or &gt; 23.5%, LEL &gt;= 10%, H2S &gt;= 10 ppm, CO &gt;= 50 ppm).
    /// Immediate audible/visual plant sirens, mandatory zone evacuation, muster point head-count.
    /// </summary>
    Evacuation = 3
}

/// <summary>
/// Direction and velocity of toxic or combustible atmospheric concentration changes over time.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GasTrendVelocity
{
    /// <summary>
    /// Sensor values fluctuate within normal baseline measurement noise.
    /// </summary>
    Stable = 1,

    /// <summary>
    /// Gas concentration exhibits a slow, steady upward drift over multiple sample intervals.
    /// </summary>
    SlowRise = 2,

    /// <summary>
    /// Rapid surge or steep upward gradient indicating imminent containment loss or ventilation failure.
    /// </summary>
    RapidClimbDangerous = 3,

    /// <summary>
    /// Gas concentration is decreasing back toward safe baseline levels.
    /// </summary>
    DecliningRecovery = 4,

    /// <summary>
    /// Unstable readings oscillating erratically across sample points.
    /// </summary>
    ErraticFluctuation = 5
}

/// <summary>
/// Standard atmospheric threshold configuration governing gas safety decisions.
/// Follows OSHA 1910.146 Confined Space and NIOSH / ACGIH chemical exposure standards.
/// </summary>
public record GasThresholdLimits(
    double OxygenSafeMin = 19.5,
    double OxygenSafeMax = 23.5,
    double OxygenActionMin = 20.0,
    double OxygenActionMax = 22.5,
    double LelSafeMax = 10.0,
    double LelActionThreshold = 5.0,
    double LelEvacuationThreshold = 10.0,
    double H2sSafeMax = 10.0,
    double H2sActionThreshold = 5.0,
    double H2sEvacuationThreshold = 10.0,
    double CoSafeMax = 25.0,
    double CoActionThreshold = 25.0,
    double CoEvacuationThreshold = 50.0,
    double VocSafeMax = 5.0,
    double VocActionThreshold = 2.0,
    double VocEvacuationThreshold = 5.0
);

/// <summary>
/// Single atmospheric telemetry sample recorded by a fixed or wireless portable gas detector.
/// </summary>
public record GasReading(
    Guid ReadingId,
    string MonitorSerialNumber,
    string ZoneCode,
    string? PermitNumber,
    DateTime TimestampUtc,
    double OxygenPercentage,
    double LelPercentage,
    double H2sPpm,
    double CoPpm,
    double? VocPpm = null,
    double? AmbientTemperatureCelsius = null
);

/// <summary>
/// Evaluation result for a specific individual gas component within a telemetry reading.
/// </summary>
public record GasChannelEvaluation(
    GasType Gas,
    double Value,
    string Unit,
    bool IsWithinSafeThreshold,
    GasAlarmLevel AlarmLevel,
    string StatusDescription
);

/// <summary>
/// Comprehensive multi-channel evaluation of a single atmospheric telemetry reading.
/// </summary>
public record GasReadingValidationResult(
    Guid ReadingId,
    DateTime TimestampUtc,
    string MonitorSerialNumber,
    string ZoneCode,
    string? PermitNumber,
    GasAlarmLevel HighestAlarmLevel,
    bool IsSafeForEntry,
    IReadOnlyList<GasChannelEvaluation> ChannelEvaluations,
    IReadOnlyList<string> AlarmMessages
);

/// <summary>
/// Dynamic trending pattern identified across sequential atmospheric sensor readings.
/// </summary>
public record GasTrendPattern(
    GasType Gas,
    string ZoneCode,
    double StartValue,
    double EndValue,
    double TotalDelta,
    double RateOfChangePerMinute,
    TimeSpan Duration,
    GasTrendVelocity Velocity,
    bool IsRisingDangerously,
    string Description,
    string RecommendedAction
);

/// <summary>
/// Statistical metrics for an individual atmospheric gas channel over a monitoring session.
/// </summary>
public record GasStatistics(
    GasType Gas,
    double Minimum,
    double Maximum,
    double Average,
    double Latest,
    string Unit
);

/// <summary>
/// Executive and operational summary of atmospheric monitoring across a plant zone.
/// </summary>
public record AtmosphericMonitoringSummary(
    string ZoneCode,
    string? PermitNumber,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    int TotalReadings,
    GasAlarmLevel PeakAlarmLevel,
    bool IsSafeToWork,
    GasStatistics OxygenStats,
    GasStatistics LelStats,
    GasStatistics H2sStats,
    GasStatistics CoStats,
    GasStatistics? VocStats,
    IReadOnlyDictionary<GasAlarmLevel, int> AlarmCounts,
    IReadOnlyList<GasTrendPattern> DetectedTrends,
    IReadOnlyList<string> SignificantEvents,
    IReadOnlyList<string> RecommendedSafetyActions,
    DateTime GeneratedAtUtc
);

#endregion

#region Service Contract

/// <summary>
/// Service interface governing real-time gas telemetry analysis, threshold verification,
/// dangerous concentration trend detection (e.g. rising H2S or combustible gas), alarm level escalation,
/// and atmospheric monitoring reporting for ClearToWork AI.
/// </summary>
public interface IGasReadingAnalyzer
{
    /// <summary>
    /// Validates an individual multi-gas reading against OSHA 1910.146 and site-specific safety thresholds.
    /// </summary>
    /// <param name="reading">The raw gas monitor telemetry reading.</param>
    /// <param name="thresholds">Optional customized gas thresholds; defaults to OSHA/NIOSH standards.</param>
    /// <returns>A structured <see cref="GasReadingValidationResult"/>.</returns>
    GasReadingValidationResult EvaluateReading(GasReading reading, GasThresholdLimits? thresholds = null);

    /// <summary>
    /// Validates a batch of gas readings and returns per-reading validation results.
    /// </summary>
    IReadOnlyList<GasReadingValidationResult> EvaluateReadings(
        IEnumerable<GasReading> readings,
        GasThresholdLimits? thresholds = null);

    /// <summary>
    /// Determines the highest alarm severity level triggered by a gas telemetry reading.
    /// </summary>
    GasAlarmLevel DetermineAlarmLevel(GasReading reading, GasThresholdLimits? thresholds = null);

    /// <summary>
    /// Checks whether all atmospheric gas concentrations in a reading satisfy entry and continuous occupancy safety criteria.
    /// </summary>
    bool IsAtmosphereSafeForEntry(GasReading reading, GasThresholdLimits? thresholds = null);

    /// <summary>
    /// Detects trend patterns across a chronologically ordered series of gas readings,
    /// identifying rapid toxic gas rises (such as rising H2S), combustible buildup, or oxygen depletion.
    /// </summary>
    /// <param name="readingsWindow">Chronologically ordered gas telemetry samples from a specific zone or device.</param>
    /// <param name="minTimeSpan">Minimum time duration required to calculate a valid rate of change.</param>
    /// <returns>A collection of detected <see cref="GasTrendPattern"/> alerts.</returns>
    IReadOnlyList<GasTrendPattern> DetectTrendingPatterns(
        IEnumerable<GasReading> readingsWindow,
        TimeSpan? minTimeSpan = null);

    /// <summary>
    /// Compiles an atmospheric monitoring summary report for a facility zone and permit.
    /// </summary>
    /// <param name="zoneCode">The plant operational zone code.</param>
    /// <param name="readings">All gas telemetry samples recorded within the monitoring period.</param>
    /// <param name="permitNumber">Optional associated permit number.</param>
    /// <param name="thresholds">Optional gas threshold configuration.</param>
    /// <returns>A complete <see cref="AtmosphericMonitoringSummary"/>.</returns>
    AtmosphericMonitoringSummary GenerateAtmosphericSummary(
        string zoneCode,
        IEnumerable<GasReading> readings,
        string? permitNumber = null,
        GasThresholdLimits? thresholds = null);
}

#endregion

#region Implementation

/// <summary>
/// Production implementation of <see cref="IGasReadingAnalyzer"/> enforcing OSHA 29 CFR 1910.146,
/// NIOSH chemical hazard guidelines, and IOGP Life-Saving Rules for atmospheric gas monitoring.
/// </summary>
public class GasReadingAnalyzer : IGasReadingAnalyzer
{
    private static readonly GasThresholdLimits DefaultThresholds = new();

    /// <inheritdoc />
    public GasReadingValidationResult EvaluateReading(GasReading reading, GasThresholdLimits? thresholds = null)
    {
        ArgumentNullException.ThrowIfNull(reading);

        var limits = thresholds ?? DefaultThresholds;
        var channels = new List<GasChannelEvaluation>();
        var alarms = new List<string>();

        // 1. Oxygen (O2) Evaluation: Safe range 19.5% - 23.5%
        var o2 = reading.OxygenPercentage;
        GasAlarmLevel o2Level;
        bool o2Safe;
        string o2Desc;

        if (o2 < limits.OxygenSafeMin)
        {
            o2Level = GasAlarmLevel.Evacuation;
            o2Safe = false;
            o2Desc = $"CRITICAL OXYGEN DEFICIENCY: O2 level is {o2:F1}% (below {limits.OxygenSafeMin}% OSHA limit). Severe asphyxiation hazard!";
            alarms.Add(o2Desc);
        }
        else if (o2 > limits.OxygenSafeMax)
        {
            o2Level = GasAlarmLevel.Evacuation;
            o2Safe = false;
            o2Desc = $"CRITICAL OXYGEN ENRICHMENT: O2 level is {o2:F1}% (above {limits.OxygenSafeMax}%). High combustion and explosion risk!";
            alarms.Add(o2Desc);
        }
        else if (o2 < limits.OxygenActionMin)
        {
            o2Level = GasAlarmLevel.Action;
            o2Safe = true;
            o2Desc = $"ACTION REQUIRED: O2 level is {o2:F1}% (approaching lower safety threshold {limits.OxygenSafeMin}%). Check forced air ventilation.";
            alarms.Add(o2Desc);
        }
        else if (o2 > limits.OxygenActionMax)
        {
            o2Level = GasAlarmLevel.Action;
            o2Safe = true;
            o2Desc = $"ACTION REQUIRED: O2 level is {o2:F1}% (approaching upper enrichment threshold {limits.OxygenSafeMax}%).";
            alarms.Add(o2Desc);
        }
        else
        {
            o2Level = GasAlarmLevel.Normal;
            o2Safe = true;
            o2Desc = $"NORMAL: O2 concentration is {o2:F1}% (acceptable range 19.5% - 23.5%).";
        }

        channels.Add(new GasChannelEvaluation(GasType.Oxygen, o2, "%", o2Safe, o2Level, o2Desc));

        // 2. Combustible Gases (LEL) Evaluation: Safe < 10% LEL
        var lel = reading.LelPercentage;
        GasAlarmLevel lelLevel;
        bool lelSafe;
        string lelDesc;

        if (lel >= limits.LelEvacuationThreshold)
        {
            lelLevel = GasAlarmLevel.Evacuation;
            lelSafe = false;
            lelDesc = $"CRITICAL LEL EVACUATION: Combustible gas is {lel:F1}% LEL (>= {limits.LelEvacuationThreshold}% limit). Explosion hazard!";
            alarms.Add(lelDesc);
        }
        else if (lel >= limits.LelActionThreshold)
        {
            lelLevel = GasAlarmLevel.Action;
            lelSafe = true;
            lelDesc = $"ACTION REQUIRED: Combustible gas is {lel:F1}% LEL (>= {limits.LelActionThreshold}% action limit). Cease hot work and ventilate.";
            alarms.Add(lelDesc);
        }
        else if (lel > 0.0)
        {
            lelLevel = GasAlarmLevel.Warning;
            lelSafe = true;
            lelDesc = $"WARNING: Minor combustible gas trace detected at {lel:F1}% LEL.";
        }
        else
        {
            lelLevel = GasAlarmLevel.Normal;
            lelSafe = true;
            lelDesc = $"NORMAL: Zero combustible hydrocarbons detected (0.0% LEL).";
        }

        channels.Add(new GasChannelEvaluation(GasType.Combustible_LEL, lel, "% LEL", lelSafe, lelLevel, lelDesc));

        // 3. Hydrogen Sulfide (H2S) Evaluation: Safe < 10 ppm
        var h2s = reading.H2sPpm;
        GasAlarmLevel h2sLevel;
        bool h2sSafe;
        string h2sDesc;

        if (h2s >= limits.H2sEvacuationThreshold)
        {
            h2sLevel = GasAlarmLevel.Evacuation;
            h2sSafe = false;
            h2sDesc = $"CRITICAL H2S EVACUATION: Sour gas is {h2s:F1} ppm (>= {limits.H2sEvacuationThreshold} ppm ceiling). Immediate olfactory fatigue and fatal toxicity risk!";
            alarms.Add(h2sDesc);
        }
        else if (h2s >= limits.H2sActionThreshold)
        {
            h2sLevel = GasAlarmLevel.Action;
            h2sSafe = true;
            h2sDesc = $"ACTION REQUIRED: H2S concentration is {h2s:F1} ppm (>= {limits.H2sActionThreshold} ppm action level). Don positive-pressure SCBA.";
            alarms.Add(h2sDesc);
        }
        else if (h2s > 0.5)
        {
            h2sLevel = GasAlarmLevel.Warning;
            h2sSafe = true;
            h2sDesc = $"WARNING: Detectable H2S odor threshold reached at {h2s:F1} ppm.";
        }
        else
        {
            h2sLevel = GasAlarmLevel.Normal;
            h2sSafe = true;
            h2sDesc = $"NORMAL: H2S toxic gas within permissible ambient background ({h2s:F1} ppm).";
        }

        channels.Add(new GasChannelEvaluation(GasType.HydrogenSulfide_H2S, h2s, "ppm", h2sSafe, h2sLevel, h2sDesc));

        // 4. Carbon Monoxide (CO) Evaluation: Safe < 25 ppm
        var co = reading.CoPpm;
        GasAlarmLevel coLevel;
        bool coSafe;
        string coDesc;

        if (co >= limits.CoEvacuationThreshold)
        {
            coLevel = GasAlarmLevel.Evacuation;
            coSafe = false;
            coDesc = $"CRITICAL CO EVACUATION: Carbon monoxide is {co:F1} ppm (>= {limits.CoEvacuationThreshold} ppm). Immediate evacuation required!";
            alarms.Add(coDesc);
        }
        else if (co >= limits.CoActionThreshold)
        {
            coLevel = GasAlarmLevel.Action;
            coSafe = true;
            coDesc = $"ACTION REQUIRED: Carbon monoxide is {co:F1} ppm (>= {limits.CoActionThreshold} ppm). Inspect combustion engines and increase airflow.";
            alarms.Add(coDesc);
        }
        else if (co > 10.0)
        {
            coLevel = GasAlarmLevel.Warning;
            coSafe = true;
            coDesc = $"WARNING: Elevated CO background at {co:F1} ppm.";
        }
        else
        {
            coLevel = GasAlarmLevel.Normal;
            coSafe = true;
            coDesc = $"NORMAL: CO concentration safe ({co:F1} ppm).";
        }

        channels.Add(new GasChannelEvaluation(GasType.CarbonMonoxide_CO, co, "ppm", coSafe, coLevel, coDesc));

        // 5. Optional VOC Evaluation
        if (reading.VocPpm.HasValue)
        {
            var voc = reading.VocPpm.Value;
            GasAlarmLevel vocLevel;
            bool vocSafe;
            string vocDesc;

            if (voc >= limits.VocEvacuationThreshold)
            {
                vocLevel = GasAlarmLevel.Evacuation;
                vocSafe = false;
                vocDesc = $"CRITICAL VOC EVACUATION: Volatile organics at {voc:F1} ppm exceed {limits.VocEvacuationThreshold} ppm limit.";
                alarms.Add(vocDesc);
            }
            else if (voc >= limits.VocActionThreshold)
            {
                vocLevel = GasAlarmLevel.Action;
                vocSafe = true;
                vocDesc = $"ACTION REQUIRED: Volatile organics elevated at {voc:F1} ppm (>= {limits.VocActionThreshold} ppm).";
                alarms.Add(vocDesc);
            }
            else
            {
                vocLevel = GasAlarmLevel.Normal;
                vocSafe = true;
                vocDesc = $"NORMAL: VOC concentration safe ({voc:F1} ppm).";
            }

            channels.Add(new GasChannelEvaluation(GasType.VolatileOrganic_VOC, voc, "ppm", vocSafe, vocLevel, vocDesc));
        }

        var highestAlarm = channels.Max(c => c.AlarmLevel);
        var isSafeForEntry = channels.All(c => c.IsWithinSafeThreshold);

        return new GasReadingValidationResult(
            ReadingId: reading.ReadingId,
            TimestampUtc: reading.TimestampUtc,
            MonitorSerialNumber: reading.MonitorSerialNumber,
            ZoneCode: reading.ZoneCode,
            PermitNumber: reading.PermitNumber,
            HighestAlarmLevel: highestAlarm,
            IsSafeForEntry: isSafeForEntry,
            ChannelEvaluations: channels,
            AlarmMessages: alarms
        );
    }

    /// <inheritdoc />
    public IReadOnlyList<GasReadingValidationResult> EvaluateReadings(
        IEnumerable<GasReading> readings,
        GasThresholdLimits? thresholds = null)
    {
        ArgumentNullException.ThrowIfNull(readings);
        return readings.Select(r => EvaluateReading(r, thresholds)).ToList();
    }

    /// <inheritdoc />
    public GasAlarmLevel DetermineAlarmLevel(GasReading reading, GasThresholdLimits? thresholds = null)
    {
        var eval = EvaluateReading(reading, thresholds);
        return eval.HighestAlarmLevel;
    }

    /// <inheritdoc />
    public bool IsAtmosphereSafeForEntry(GasReading reading, GasThresholdLimits? thresholds = null)
    {
        var eval = EvaluateReading(reading, thresholds);
        return eval.IsSafeForEntry;
    }

    /// <inheritdoc />
    public IReadOnlyList<GasTrendPattern> DetectTrendingPatterns(
        IEnumerable<GasReading> readingsWindow,
        TimeSpan? minTimeSpan = null)
    {
        ArgumentNullException.ThrowIfNull(readingsWindow);

        var list = readingsWindow.OrderBy(r => r.TimestampUtc).ToList();
        if (list.Count < 3)
        {
            return Array.Empty<GasTrendPattern>();
        }

        var patterns = new List<GasTrendPattern>();
        var first = list.First();
        var last = list.Last();
        var totalMinutes = (last.TimestampUtc - first.TimestampUtc).TotalMinutes;

        var minMinutes = minTimeSpan?.TotalMinutes ?? 1.0;
        if (totalMinutes < minMinutes)
        {
            return Array.Empty<GasTrendPattern>();
        }

        var zoneCode = list.Last().ZoneCode;

        // 1. Analyze H2S Trend (Rising H2S detection)
        var h2sStart = first.H2sPpm;
        var h2sEnd = last.H2sPpm;
        var h2sDelta = h2sEnd - h2sStart;
        var h2sRate = totalMinutes > 0 ? h2sDelta / totalMinutes : 0.0;

        // Check for persistent monotonic rise in H2S
        var consecutiveH2sRises = 0;
        for (var i = 1; i < list.Count; i++)
        {
            if (list[i].H2sPpm > list[i - 1].H2sPpm)
            {
                consecutiveH2sRises++;
            }
        }

        var isH2sRisingConsistently = consecutiveH2sRises >= list.Count / 2 && h2sDelta > 0.5;

        if (h2sRate >= 0.5 || (h2sDelta >= 2.0 && isH2sRisingConsistently))
        {
            var isDangerous = h2sEnd >= 5.0 || h2sRate >= 1.0;
            var velocity = h2sRate >= 1.0 ? GasTrendVelocity.RapidClimbDangerous : GasTrendVelocity.SlowRise;

            patterns.Add(new GasTrendPattern(
                Gas: GasType.HydrogenSulfide_H2S,
                ZoneCode: zoneCode,
                StartValue: h2sStart,
                EndValue: h2sEnd,
                TotalDelta: Math.Round(h2sDelta, 2),
                RateOfChangePerMinute: Math.Round(h2sRate, 3),
                Duration: last.TimestampUtc - first.TimestampUtc,
                Velocity: velocity,
                IsRisingDangerously: isDangerous,
                Description: $"TREND DETECTED: Rising H2S concentration in {zoneCode} from {h2sStart:F1} to {h2sEnd:F1} ppm (+{h2sDelta:F1} ppm over {totalMinutes:F1} min, Rate: {h2sRate:F2} ppm/min).",
                RecommendedAction: isDangerous
                    ? "IMMEDIATE ACTION: Order hot work shutdown, don respiratory protection, and dispatch gas testing technician."
                    : "MONITOR: Increase telemetry polling rate and check zone boundary sniffers."
            ));
        }

        // 2. Analyze LEL Trend (Combustible Buildup)
        var lelStart = first.LelPercentage;
        var lelEnd = last.LelPercentage;
        var lelDelta = lelEnd - lelStart;
        var lelRate = totalMinutes > 0 ? lelDelta / totalMinutes : 0.0;

        if (lelRate >= 0.5 || (lelDelta >= 2.0 && lelEnd >= 3.0))
        {
            var isDangerous = lelEnd >= 5.0 || lelRate >= 1.5;
            var velocity = lelRate >= 1.5 ? GasTrendVelocity.RapidClimbDangerous : GasTrendVelocity.SlowRise;

            patterns.Add(new GasTrendPattern(
                Gas: GasType.Combustible_LEL,
                ZoneCode: zoneCode,
                StartValue: lelStart,
                EndValue: lelEnd,
                TotalDelta: Math.Round(lelDelta, 2),
                RateOfChangePerMinute: Math.Round(lelRate, 3),
                Duration: last.TimestampUtc - first.TimestampUtc,
                Velocity: velocity,
                IsRisingDangerously: isDangerous,
                Description: $"TREND DETECTED: Rising combustible gas in {zoneCode} from {lelStart:F1}% to {lelEnd:F1}% LEL (+{lelDelta:F1}% LEL over {totalMinutes:F1} min).",
                RecommendedAction: isDangerous
                    ? "CRITICAL: De-energize hot work ignition sources, start explosion-proof extractors, verify pipeline isolation."
                    : "ADVISORY: Inspect pipe flanges and bleed valves in vicinity."
            ));
        }

        // 3. Analyze Oxygen Depletion Trend (Asphyxiation Risk)
        var o2Start = first.OxygenPercentage;
        var o2End = last.OxygenPercentage;
        var o2Delta = o2End - o2Start; // Negative if depleting
        var o2Rate = totalMinutes > 0 ? o2Delta / totalMinutes : 0.0;

        if (o2Delta <= -0.5 && o2Rate <= -0.1)
        {
            var isDangerous = o2End <= 20.0;
            var velocity = o2Rate <= -0.3 ? GasTrendVelocity.RapidClimbDangerous : GasTrendVelocity.SlowRise;

            patterns.Add(new GasTrendPattern(
                Gas: GasType.Oxygen,
                ZoneCode: zoneCode,
                StartValue: o2Start,
                EndValue: o2End,
                TotalDelta: Math.Round(o2Delta, 2),
                RateOfChangePerMinute: Math.Round(o2Rate, 3),
                Duration: last.TimestampUtc - first.TimestampUtc,
                Velocity: velocity,
                IsRisingDangerously: isDangerous,
                Description: $"TREND DETECTED: Oxygen depletion in {zoneCode} dropping from {o2Start:F1}% to {o2End:F1}% ({o2Delta:F1}% over {totalMinutes:F1} min).",
                RecommendedAction: "Verify fresh air intake blowers; check for nitrogen or inert gas leakage into confined space."
            ));
        }

        return patterns;
    }

    /// <inheritdoc />
    public AtmosphericMonitoringSummary GenerateAtmosphericSummary(
        string zoneCode,
        IEnumerable<GasReading> readings,
        string? permitNumber = null,
        GasThresholdLimits? thresholds = null)
    {
        ArgumentNullException.ThrowIfNull(readings);

        var list = readings.OrderBy(r => r.TimestampUtc).ToList();
        var now = DateTime.UtcNow;

        if (list.Count == 0)
        {
            return new AtmosphericMonitoringSummary(
                ZoneCode: zoneCode,
                PermitNumber: permitNumber,
                StartTimeUtc: now,
                EndTimeUtc: now,
                TotalReadings: 0,
                PeakAlarmLevel: GasAlarmLevel.Normal,
                IsSafeToWork: false,
                OxygenStats: new GasStatistics(GasType.Oxygen, 0, 0, 0, 0, "%"),
                LelStats: new GasStatistics(GasType.Combustible_LEL, 0, 0, 0, 0, "% LEL"),
                H2sStats: new GasStatistics(GasType.HydrogenSulfide_H2S, 0, 0, 0, 0, "ppm"),
                CoStats: new GasStatistics(GasType.CarbonMonoxide_CO, 0, 0, 0, 0, "ppm"),
                VocStats: null,
                AlarmCounts: new Dictionary<GasAlarmLevel, int> { [GasAlarmLevel.Normal] = 0 },
                DetectedTrends: Array.Empty<GasTrendPattern>(),
                SignificantEvents: new[] { "No gas telemetry readings recorded for this evaluation window." },
                RecommendedSafetyActions: new[] { "Deploy certified atmospheric gas monitor before entering zone." },
                GeneratedAtUtc: now
            );
        }

        var evaluatedReadings = EvaluateReadings(list, thresholds);
        var detectedTrends = DetectTrendingPatterns(list);

        var alarmCounts = new Dictionary<GasAlarmLevel, int>
        {
            [GasAlarmLevel.Normal] = 0,
            [GasAlarmLevel.Warning] = 0,
            [GasAlarmLevel.Action] = 0,
            [GasAlarmLevel.Evacuation] = 0
        };

        foreach (var eval in evaluatedReadings)
        {
            alarmCounts[eval.HighestAlarmLevel]++;
        }

        var peakAlarm = evaluatedReadings.Max(e => e.HighestAlarmLevel);
        var latestReading = evaluatedReadings.Last();
        var isSafeToWork = latestReading.IsSafeForEntry && peakAlarm != GasAlarmLevel.Evacuation;

        // Calculate statistics per gas channel
        var o2Stats = new GasStatistics(
            Gas: GasType.Oxygen,
            Minimum: Math.Round(list.Min(r => r.OxygenPercentage), 1),
            Maximum: Math.Round(list.Max(r => r.OxygenPercentage), 1),
            Average: Math.Round(list.Average(r => r.OxygenPercentage), 1),
            Latest: Math.Round(list.Last().OxygenPercentage, 1),
            Unit: "%"
        );

        var lelStats = new GasStatistics(
            Gas: GasType.Combustible_LEL,
            Minimum: Math.Round(list.Min(r => r.LelPercentage), 1),
            Maximum: Math.Round(list.Max(r => r.LelPercentage), 1),
            Average: Math.Round(list.Average(r => r.LelPercentage), 1),
            Latest: Math.Round(list.Last().LelPercentage, 1),
            Unit: "% LEL"
        );

        var h2sStats = new GasStatistics(
            Gas: GasType.HydrogenSulfide_H2S,
            Minimum: Math.Round(list.Min(r => r.H2sPpm), 1),
            Maximum: Math.Round(list.Max(r => r.H2sPpm), 1),
            Average: Math.Round(list.Average(r => r.H2sPpm), 1),
            Latest: Math.Round(list.Last().H2sPpm, 1),
            Unit: "ppm"
        );

        var coStats = new GasStatistics(
            Gas: GasType.CarbonMonoxide_CO,
            Minimum: Math.Round(list.Min(r => r.CoPpm), 1),
            Maximum: Math.Round(list.Max(r => r.CoPpm), 1),
            Average: Math.Round(list.Average(r => r.CoPpm), 1),
            Latest: Math.Round(list.Last().CoPpm, 1),
            Unit: "ppm"
        );

        GasStatistics? vocStats = null;
        var vocReadings = list.Where(r => r.VocPpm.HasValue).Select(r => r.VocPpm!.Value).ToList();
        if (vocReadings.Count > 0)
        {
            vocStats = new GasStatistics(
                Gas: GasType.VolatileOrganic_VOC,
                Minimum: Math.Round(vocReadings.Min(), 1),
                Maximum: Math.Round(vocReadings.Max(), 1),
                Average: Math.Round(vocReadings.Average(), 1),
                Latest: Math.Round(vocReadings.Last(), 1),
                Unit: "ppm"
            );
        }

        // Aggregate significant events and messages
        var significantEvents = evaluatedReadings
            .SelectMany(e => e.AlarmMessages)
            .Distinct()
            .ToList();

        var actions = new List<string>();
        if (peakAlarm == GasAlarmLevel.Evacuation)
        {
            actions.Add("MANDATORY EVACUATION: Life-safety gas thresholds breached during session. Zone must be ventilated and re-tested before re-entry.");
        }
        else if (peakAlarm == GasAlarmLevel.Action)
        {
            actions.Add("ACTION REQUIRED: Investigate gas source, increase forced mechanical ventilation, verify respirator readiness.");
        }
        else if (detectedTrends.Any(t => t.IsRisingDangerously))
        {
            actions.Add("WARNING: Dangerous rising gas trend detected. Maintain active continuous monitoring and alert Performing Authority.");
        }
        else
        {
            actions.Add("ATMOSPHERE COMPLIANT: All atmospheric parameters remain within OSHA safe limits for ongoing permit activities.");
        }

        return new AtmosphericMonitoringSummary(
            ZoneCode: zoneCode,
            PermitNumber: permitNumber,
            StartTimeUtc: list.First().TimestampUtc,
            EndTimeUtc: list.Last().TimestampUtc,
            TotalReadings: list.Count,
            PeakAlarmLevel: peakAlarm,
            IsSafeToWork: isSafeToWork,
            OxygenStats: o2Stats,
            LelStats: lelStats,
            H2sStats: h2sStats,
            CoStats: coStats,
            VocStats: vocStats,
            AlarmCounts: alarmCounts,
            DetectedTrends: detectedTrends,
            SignificantEvents: significantEvents,
            RecommendedSafetyActions: actions,
            GeneratedAtUtc: now
        );
    }
}

#endregion
