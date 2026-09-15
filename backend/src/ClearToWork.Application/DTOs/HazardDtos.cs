namespace ClearToWork.Application.DTOs;

public record ZoneDto(
    Guid Id,
    Guid SiteId,
    string Code,
    string Name,
    decimal Latitude,
    decimal Longitude,
    double RadiusMeters,
    string QrCodePayload,
    bool IsActive,
    List<string> AdjacentZoneCodes
);

public record ZoneConflictCheckRequest(
    Guid ZoneId,
    string HazardCode,
    DateTime StartTime,
    DateTime EndTime
);

public record ConflictItem(
    string ConflictingPermitNumber,
    string ZoneCode,
    string HazardCode,
    string ReasonRuleCode,
    string Explanation,
    DateTime ActiveStartTime,
    DateTime ActiveEndTime
);

public record ZoneConflictCheckResponse(
    bool HasConflict,
    List<ConflictItem> Conflicts,
    string? SuggestedAlternativeTimeWindow
);

public record WeatherForecastDto(
    decimal Latitude,
    decimal Longitude,
    double TemperatureC,
    double WindSpeedKmh,
    double WindGustsKmh,
    double PrecipitationProbability,
    bool IsRainExpected,
    bool IsSafeForHotWork,
    bool IsSafeForHeightWork,
    string Summary
);

public record AnalyticsSafetySummaryDto(
    int TotalPermitsIssued,
    int TotalPermitsRefused,
    int ActivePermitsCount,
    double MeanTimeToApprovalHours,
    Dictionary<string, int> RefusalCausesRanking,
    Dictionary<string, int> PermitsByZoneDistribution
);

// ─── Student 4: Safety Observations DTOs ─────────────────────────────────────
public record ObservationDto(
    Guid Id,
    Guid ZoneId,
    string ZoneCode,
    string ZoneName,
    Guid ReportedByUserId,
    string ReporterName,
    string Category,
    string Description,
    DateTime LoggedAt,
    DateTime CreatedAt
);

public record CreateObservationRequest(
    Guid ZoneId,
    string Category,
    string Description
);

public record UpdateObservationRequest(
    Guid? ZoneId,
    string Category,
    string Description
);

// ─── Student 4: Rulebook & Hazard Types DTOs ─────────────────────────────────
public record ControlMeasureDto(
    Guid Id,
    Guid HazardTypeId,
    string Code,
    string RequirementDescription,
    bool IsMandatory
);

public record HazardTypeDto(
    Guid Id,
    string Code,
    string Name,
    string SeverityLevel,
    double? MaxWindSpeedKmh,
    bool ProhibitedInRain,
    List<ControlMeasureDto> ControlMeasures
);

public record CreateHazardTypeRequest(
    string Code,
    string Name,
    string SeverityLevel,
    double? MaxWindSpeedKmh,
    bool ProhibitedInRain
);

public record UpdateHazardTypeRequest(
    string Name,
    string SeverityLevel,
    double? MaxWindSpeedKmh,
    bool ProhibitedInRain
);

public record CreateControlMeasureRequest(
    Guid HazardTypeId,
    string Code,
    string RequirementDescription,
    bool IsMandatory
);

public record UpdateControlMeasureRequest(
    string RequirementDescription,
    bool IsMandatory
);

// ─── Student 4: Incompatibility & SIMOPS Matrix DTOs ────────────────────────
public record IncompatibilityRuleDto(
    Guid Id,
    string RuleCode,
    Guid PrimaryHazardId,
    string PrimaryHazardCode,
    string PrimaryHazardName,
    Guid ConflictingHazardId,
    string ConflictingHazardCode,
    string ConflictingHazardName,
    string Reason,
    bool AppliesToAdjacentZones
);

public record CreateIncompatibilityRuleRequest(
    string RuleCode,
    Guid PrimaryHazardId,
    Guid ConflictingHazardId,
    string Reason,
    bool AppliesToAdjacentZones
);

public record UpdateIncompatibilityRuleRequest(
    string Reason,
    bool AppliesToAdjacentZones
);

public record ZoneAdjacencyDto(
    Guid ZoneId,
    string ZoneCode,
    string ZoneName,
    Guid AdjacentZoneId,
    string AdjacentZoneCode,
    string AdjacentZoneName
);

public record AddZoneAdjacencyRequest(
    Guid ZoneId,
    Guid AdjacentZoneId
);