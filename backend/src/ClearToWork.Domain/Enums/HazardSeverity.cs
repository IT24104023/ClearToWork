namespace ClearToWork.Domain.Enums;

/// <summary>
/// Defines standardized hazard severity levels for Permit-to-Work (PTW) safety risk assessments
/// and SIMOPS (Simultaneous Operations) conflict evaluations in oil & gas facilities.
/// </summary>
public enum HazardSeverity
{
    /// <summary>
    /// Negligible or no operational hazard identified.
    /// Standard baseline site PPE and general safety inductions apply.
    /// <example>Routine office walk-around, visual inspection of de-energized signage, housekeeping in low-risk laydown yard.</example>
    /// </summary>
    None = 0,

    /// <summary>
    /// Low severity hazard with minimal potential for personal injury or asset disruption.
    /// Risk is controllable via routine site procedures and standard task risk assessments (TRA).
    /// <example>Cold work in non-hazardous open areas, non-powered hand tool usage, light filter change on isolated low-pressure HVAC unit.</example>
    /// </summary>
    Low = 1,

    /// <summary>
    /// Moderate severity hazard capable of causing minor injury, localized spillage, or minor equipment damage.
    /// Requires verified toolbox talk, localized barricading, and certified tools.
    /// <example>Working at height between 2m and 5m with harness, operating small pneumatic breakers, low-pressure water jetting (&lt; 200 bar).</example>
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High severity hazard with credible potential for severe injury, occupational illness, or equipment failure.
    /// Mandatory physical barrier controls, dedicated safety watcher, and area supervisor clearance required.
    /// <example>Confined space entry, hot work (welding/cutting) near flammables, scaffolding erection adjacent to live hydrocarbon piping.</example>
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical severity hazard capable of causing permanent disability, single fatality, or major asset damage.
    /// Requires HSE Manager pre-authorization, continuous gas monitoring, positive physical isolations (LOTO), and emergency standby crew.
    /// <example>Live plant intrusive maintenance, radioactive radiography (NDT) testing, heavy tandem crane lifting over process equipment.</example>
    /// </summary>
    Critical = 4,

    /// <summary>
    /// Catastrophic or extreme hazard presenting imminent danger to human life, catastrophic asset loss, or environmental devastation.
    /// Immediate Stop Work Authority (SWA) applies if concurrent operations clash. Requires Operations Director sign-off.
    /// <example>Hot tapping into pressurized sour gas (H2S) headers, high-pressure nitrogen purging of live reactor vessels, simultaneous diving and subsea lifting.</example>
    /// </summary>
    Extreme = 5
}

/// <summary>
/// Extension helper methods for <see cref="HazardSeverity"/> to facilitate safety matrix decisions,
/// UI color-coding, and SIMOPS conflict threshold calculations.
/// </summary>
public static class HazardSeverityExtensions
{
    /// <summary>
    /// Determines whether the hazard level mandates continuous atmospheric gas testing.
    /// </summary>
    public static bool RequiresContinuousGasMonitoring(this HazardSeverity severity) =>
        severity >= HazardSeverity.High;

    /// <summary>
    /// Determines whether dual authorization (Area Authority + Site Safety Lead) is required prior to permit activation.
    /// </summary>
    public static bool RequiresDualSignOff(this HazardSeverity severity) =>
        severity >= HazardSeverity.Critical;

    /// <summary>
    /// Determines if concurrent operations with this severity level trigger automatic SIMOPS conflict freeze.
    /// </summary>
    public static bool IsStopWorkThreshold(this HazardSeverity severity) =>
        severity == HazardSeverity.Extreme;

    /// <summary>
    /// Returns the standard HSE color code (hex) for dashboard badges and GIS overlay maps.
    /// </summary>
    public static string GetHexColor(this HazardSeverity severity) => severity switch
    {
        HazardSeverity.None => "#9E9E9E",      // Gray
        HazardSeverity.Low => "#4CAF50",       // Green
        HazardSeverity.Medium => "#FF9800",    // Amber / Orange
        HazardSeverity.High => "#F44336",      // Red
        HazardSeverity.Critical => "#9C27B0",  // Purple
        HazardSeverity.Extreme => "#B71C1C",   // Deep Crimson
        _ => "#757575"
    };

    /// <summary>
    /// Computes the escalated severity level when two interacting hazards occur concurrently in the same zone.
    /// </summary>
    public static HazardSeverity Escalate(this HazardSeverity primary, HazardSeverity secondary)
    {
        var maxSeverity = (HazardSeverity)Math.Max((int)primary, (int)secondary);

        // If both hazards are at least Medium, escalate by one level (capped at Extreme)
        if (primary >= HazardSeverity.Medium && secondary >= HazardSeverity.Medium)
        {
            var escalated = (int)maxSeverity + 1;
            return (HazardSeverity)Math.Min(escalated, (int)HazardSeverity.Extreme);
        }

        return maxSeverity;
    }
}
