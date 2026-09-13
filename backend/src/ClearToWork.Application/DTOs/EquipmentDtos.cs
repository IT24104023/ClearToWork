namespace ClearToWork.Application.DTOs;

public record AssetDto(
    Guid Id,
    string AssetTag,
    string Name,
    string Category,
    string Status,
    Guid? CurrentZoneId,
    bool IsCalibrationValid,
    DateTime? NextCalibrationDate,
    bool IsInspectionValid,
    DateTime? NextInspectionDate
);

public record EquipmentReadinessRequest(
    List<Guid> AssetIds,
    Guid ZoneId,
    DateTime StartTime,
    DateTime EndTime
);

public record AssetReadinessResult(
    Guid AssetId,
    string AssetTag,
    string Name,
    bool IsReady,
    List<string> UnreadinessReasons
);

public record EquipmentReadinessResponse(
    bool AllReady,
    List<AssetReadinessResult> Results,
    List<AssetDto> RecommendedReplacements
);

public record IsolationPointDto(
    Guid Id,
    Guid ZoneId,
    string TagIdentifier,
    string Description,
    string Type,
    string State,
    string? LockedByUserId,
    DateTime? LockedAt
);
