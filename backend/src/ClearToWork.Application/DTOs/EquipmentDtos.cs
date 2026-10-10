using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

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

public record CreateAssetRequest(
    string AssetTag,
    string Name,
    string Category,
    Guid? CurrentZoneId
);

public record UpdateAssetRequest(
    string Name,
    string Category,
    string Status,
    Guid? CurrentZoneId
);

public class CreateInspectionRequest
{
    public DateTime? InspectionDate { get; set; }
    public DateTime? NextInspectionDate { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public bool? Passed { get; set; }
    public bool? IsPassed { get; set; }
    public string? Notes { get; set; }

    public bool EffectivePassed => IsPassed ?? Passed ?? true;
    public DateTime EffectiveInspectionDate => InspectionDate ?? DateTime.UtcNow;
    public DateTime EffectiveNextInspectionDate => NextInspectionDate ?? DateTime.UtcNow.AddMonths(6);
}

public class CreateCalibrationRequest
{
    public DateTime? CalibrationDate { get; set; }
    public DateTime? NextCalibrationDate { get; set; }
    public string CalibratedBy { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public bool? PassStatus { get; set; }
    public bool? IsPassed { get; set; }

    public bool EffectivePassStatus => IsPassed ?? PassStatus ?? true;
    public DateTime EffectiveCalibrationDate => CalibrationDate ?? DateTime.UtcNow;
    public DateTime EffectiveNextCalibrationDate => NextCalibrationDate ?? DateTime.UtcNow.AddMonths(12);
}

public record CreateIsolationPointRequest(
    Guid ZoneId,
    string TagIdentifier,
    string Description,
    string Type,
    string State
);

public record UpdateIsolationPointStateRequest(
    string State,
    string? LockedByUserId
);