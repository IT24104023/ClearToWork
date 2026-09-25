using ClearToWork.Domain.Enums;

namespace ClearToWork.Infrastructure.Services;

/// <summary>
/// Categories of SIMOPS conflict resolution strategies recognized by the ClearToWork AI expert system.
/// </summary>
public enum ResolutionStrategyType
{
    /// <summary>
    /// Reschedule one or both activities to eliminate temporal overlap.
    /// </summary>
    TimeShift = 1,

    /// <summary>
    /// Increase spatial buffer distance or subdivide zone perimeters to isolate activities.
    /// </summary>
    SpatialSeparation = 2,

    /// <summary>
    /// Deploy engineering barriers, habitats, or continuous detectors to suppress interaction risk.
    /// </summary>
    MitigationControl = 3,

    /// <summary>
    /// Establish formal dependency order where one permit proceeds only after the other completes.
    /// </summary>
    ActivitySequencing = 4
}

/// <summary>
/// Operational context describing a detected SIMOPS collision between two permits.
/// </summary>
public record SimopsConflictContext(
    string PrimaryPermitNumber,
    string PrimaryHazardCode,
    string PrimaryHazardName,
    DateTime PrimaryStart,
    DateTime PrimaryEnd,
    string ConflictingPermitNumber,
    string ConflictingHazardCode,
    string ConflictingHazardName,
    DateTime ConflictingStart,
    DateTime ConflictingEnd,
    string ZoneCode,
    string ZoneName,
    bool IsAdjacentZone,
    HazardSeverity ConflictSeverity,
    double ZoneRadiusMeters = 50.0
);

/// <summary>
/// Actionable resolution recommendation produced by <see cref="IConflictResolutionEngine"/>.
/// </summary>
public record ResolutionSuggestion(
    Guid ResolutionId,
    ResolutionStrategyType Strategy,
    string Title,
    string Description,
    double FeasibilityScore,
    double SafetyScore,
    int PriorityRank,
    int EstimatedDelayMinutes,
    IReadOnlyList<string> RequiredControls,
    IReadOnlyDictionary<string, string> ActionParameters
)
{
    /// <summary>
    /// Combined weighted index balancing safety assurance (60%) and operational convenience (40%).
    /// </summary>
    public double OverallScore => Math.Round((SafetyScore * 0.6) + (FeasibilityScore * 0.4), 3);
}

/// <summary>
/// Interface for the SIMOPS conflict resolution engine.
/// </summary>
public interface IConflictResolutionEngine
{
    /// <summary>
    /// Evaluates a SIMOPS conflict and returns a ranked list of recommended resolution strategies.
    /// </summary>
    IReadOnlyList<ResolutionSuggestion> GenerateRankedResolutions(SimopsConflictContext context);
}

/// <summary>
/// Expert resolution engine for SIMOPS incompatibilities in offshore and refinery facilities.
/// Generates ranked recommendations across time shifting, spatial barriers, active mitigation controls,
/// and workflow sequencing.
/// </summary>
public class ConflictResolutionEngine : IConflictResolutionEngine
{
    /// <summary>
    /// Analyzes conflict parameters and generates prioritized resolution options.
    /// </summary>
    public IReadOnlyList<ResolutionSuggestion> GenerateRankedResolutions(SimopsConflictContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var suggestions = new List<ResolutionSuggestion>
        {
            GenerateTimeShiftSuggestion(context),
            GenerateSpatialSeparationSuggestion(context),
            GenerateMitigationControlSuggestion(context),
            GenerateActivitySequencingSuggestion(context)
        };

        // Filter out null or infeasible suggestions, then rank by overall weighted score descending
        var ranked = suggestions
            .Where(s => s.FeasibilityScore > 0.1)
            .OrderByDescending(s => s.OverallScore)
            .ThenBy(s => s.EstimatedDelayMinutes)
            .ToList();

        // Assign 1-based ranks
        return ranked
            .Select((s, index) => s with { PriorityRank = index + 1 })
            .ToList();
    }

    private static ResolutionSuggestion GenerateTimeShiftSuggestion(SimopsConflictContext context)
    {
        // Calculate earliest non-overlapping slot after conflicting activity finishes, including 30m safety buffer
        var clearanceBufferMinutes = 30;
        var proposedNewStart = context.ConflictingEnd.AddMinutes(clearanceBufferMinutes);
        var duration = context.PrimaryEnd - context.PrimaryStart;
        var proposedNewEnd = proposedNewStart + duration;

        var delayMinutes = Math.Max(0, (int)(proposedNewStart - context.PrimaryStart).TotalMinutes);

        // Feasibility decreases as delay increases (>4 hours incurs operational penalties)
        double feasibility = delayMinutes switch
        {
            <= 60 => 0.95,
            <= 180 => 0.85,
            <= 360 => 0.70,
            <= 720 => 0.50,
            _ => 0.30
        };

        // Time-shifting has near-perfect safety since concurrent operations are physically eliminated
        double safetyScore = 0.98;

        var parameters = new Dictionary<string, string>
        {
            ["RescheduledStartTime"] = proposedNewStart.ToString("yyyy-MM-dd HH:mm"),
            ["RescheduledEndTime"] = proposedNewEnd.ToString("yyyy-MM-dd HH:mm"),
            ["DelayMinutes"] = delayMinutes.ToString(),
            ["TargetPermit"] = context.PrimaryPermitNumber
        };

        return new ResolutionSuggestion(
            ResolutionId: Guid.NewGuid(),
            Strategy: ResolutionStrategyType.TimeShift,
            Title: "Reschedule Primary Activity After Conflict Window",
            Description: $"Postpone {context.PrimaryPermitNumber} ({context.PrimaryHazardName}) to start at {proposedNewStart:HH:mm} " +
                         $"({clearanceBufferMinutes}m after {context.ConflictingPermitNumber} finishes at {context.ConflictingEnd:HH:mm}).",
            FeasibilityScore: feasibility,
            SafetyScore: safetyScore,
            PriorityRank: 0,
            EstimatedDelayMinutes: delayMinutes,
            RequiredControls: new[] { "Shift handover notification to Area Authority", "Pre-work atmospheric re-test prior to delayed start" },
            ActionParameters: parameters
        );
    }

    private static ResolutionSuggestion GenerateSpatialSeparationSuggestion(SimopsConflictContext context)
    {
        double bufferExtensionMeters = context.IsAdjacentZone ? 25.0 : 40.0;
        double feasibility = context.IsAdjacentZone ? 0.85 : 0.60;
        double safetyScore = context.IsAdjacentZone ? 0.90 : 0.75;

        var controls = new List<string>
        {
            $"Demarcate mandatory {bufferExtensionMeters:F0}m physical exclusion perimeter with high-visibility barricade tape and flashers",
            "Post dedicated safety sentry at zone intersection boundary",
            "Issue acoustic wind-drift warning to personnel in downwind sectors"
        };

        var parameters = new Dictionary<string, string>
        {
            ["RecommendedBufferMeters"] = bufferExtensionMeters.ToString("F1"),
            ["RequiresPhysicalBarricade"] = "true",
            ["SentryRequired"] = "true"
        };

        return new ResolutionSuggestion(
            ResolutionId: Guid.NewGuid(),
            Strategy: ResolutionStrategyType.SpatialSeparation,
            Title: "Establish Dynamic Spatial Buffer & Physical Exclusion Zone",
            Description: $"Enforce an extended {bufferExtensionMeters:F0}m safety separation boundary between {context.ZoneName} " +
                         $"and adjacent operations, preventing overlapping hazardous vapor or ignition envelopes.",
            FeasibilityScore: feasibility,
            SafetyScore: safetyScore,
            PriorityRank: 0,
            EstimatedDelayMinutes: 15, // Minor setup delay for barrier placement
            RequiredControls: controls,
            ActionParameters: parameters
        );
    }

    private static ResolutionSuggestion GenerateMitigationControlSuggestion(SimopsConflictContext context)
    {
        var controls = new List<string>();
        double safetyScore = 0.88;
        double feasibility = 0.90; // High feasibility as both activities can proceed simultaneously

        // Select hazard-specific engineered barriers
        if (context.PrimaryHazardCode.Contains("HOT", StringComparison.OrdinalIgnoreCase) ||
            context.ConflictingHazardCode.Contains("HOT", StringComparison.OrdinalIgnoreCase))
        {
            controls.Add("Install certified fire-retardant welding habitat (pressurized containment tent)");
            controls.Add("Position continuous multigas (LEL/H2S/O2/CO) optical detector with audible/visual horn");
            controls.Add("Equip standby Fire Watcher with dedicated 50kg dry chemical cart & charged fire hose");
        }
        else if (context.PrimaryHazardCode.Contains("CONFINED", StringComparison.OrdinalIgnoreCase))
        {
            controls.Add("Deploy forced-draft continuous ventilation extractor with spark-arrested ducting");
            controls.Add("Station designated Standby Attendant equipped with tripod retrieval system");
        }
        else
        {
            controls.Add("Erect heavy-duty non-combustible flash curtains between work fronts");
            controls.Add("Hourly atmospheric sampling verified by Area Authority");
        }

        var parameters = new Dictionary<string, string>
        {
            ["BarrierType"] = "Pressurized Habitat / Fire-Retardant Screening",
            ["ContinuousGasMonitoring"] = "true",
            ["DedicatedFireWatch"] = "true"
        };

        return new ResolutionSuggestion(
            ResolutionId: Guid.NewGuid(),
            Strategy: ResolutionStrategyType.MitigationControl,
            Title: "Deploy Engineered Containment & Continuous Atmospheric Monitoring",
            Description: $"Authorize simultaneous execution under enhanced safeguard controls: install certified positive-pressure " +
                         $"containment habitat and continuous gas telemetry between {context.PrimaryPermitNumber} and {context.ConflictingPermitNumber}.",
            FeasibilityScore: feasibility,
            SafetyScore: safetyScore,
            PriorityRank: 0,
            EstimatedDelayMinutes: 30, // Time required to inspect habitat and verify atmospheric clear
            RequiredControls: controls,
            ActionParameters: parameters
        );
    }

    private static ResolutionSuggestion GenerateActivitySequencingSuggestion(SimopsConflictContext context)
    {
        // Activity sequencing enforces that one activity completes first
        var totalConflictDuration = (int)(context.ConflictingEnd - context.PrimaryStart).TotalMinutes;
        var delayMinutes = Math.Max(15, totalConflictDuration);

        double feasibility = 0.75;
        double safetyScore = 0.95;

        var controls = new List<string>
        {
            $"Link Permit {context.PrimaryPermitNumber} as strict downstream dependent of Permit {context.ConflictingPermitNumber}",
            "Mandate formal Area Authority closeout inspection of prior permit before unlocking dependent work front",
            "Perform baseline 4-gas test before contractor handover"
        };

        var parameters = new Dictionary<string, string>
        {
            ["PrerequisitePermit"] = context.ConflictingPermitNumber,
            ["DependentPermit"] = context.PrimaryPermitNumber,
            ["HandoverGasTestRequired"] = "true"
        };

        return new ResolutionSuggestion(
            ResolutionId: Guid.NewGuid(),
            Strategy: ResolutionStrategyType.ActivitySequencing,
            Title: "Enforce Sequential Workflow Handover",
            Description: $"Enforce dependency lock: {context.PrimaryPermitNumber} remains in 'Pending Prerequisite' status until " +
                         $"{context.ConflictingPermitNumber} completes operations and achieves verified Area Authority sign-off.",
            FeasibilityScore: feasibility,
            SafetyScore: safetyScore,
            PriorityRank: 0,
            EstimatedDelayMinutes: delayMinutes,
            RequiredControls: controls,
            ActionParameters: parameters
        );
    }
}
