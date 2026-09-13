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
