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

public class CheckEquipmentTagsRequest
{
    public List<string> AssetTags { get; set; } = new();
}


public class IsolationPointDto
{
    public Guid Id { get; set; }
    public Guid ZoneId { get; set; }
    public string TagIdentifier { get; set; } = string.Empty;
    public string Code => TagIdentifier;
    public string Description { get; set; } = string.Empty;
    public string Name => Description;
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string CurrentState => State == "LockedOut" || State == "LockedIsolated" ? "LockedOut" : (State == "TaggedOut" ? "TaggedOut" : "Open");
    public string? LockedByUserId { get; set; }
    public DateTime? LockedAt { get; set; }
    public string? Notes { get; set; }

    public IsolationPointDto() { }

    public IsolationPointDto(
        Guid id,
        Guid zoneId,
        string tagIdentifier,
        string description,
        string type,
        string state,
        string? lockedByUserId = null,
        DateTime? lockedAt = null,
        string? notes = null)
    {
        Id = id;
        ZoneId = zoneId;
        TagIdentifier = tagIdentifier;
        Description = description;
        Type = type;
        State = state;
        LockedByUserId = lockedByUserId;
        LockedAt = lockedAt;
        Notes = notes;
    }
}

public class CreateAssetRequest
{
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Status { get; set; }
    public Guid? CurrentZoneId { get; set; }
    public Guid? ZoneId { get => CurrentZoneId; set => CurrentZoneId = value; }

    public CreateAssetRequest() { }

    public CreateAssetRequest(string assetTag, string name, string category, Guid? currentZoneId = null)
    {
        AssetTag = assetTag;
        Name = name;
        Category = category;
        CurrentZoneId = currentZoneId;
    }
}

public class UpdateAssetRequest
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? CurrentZoneId { get; set; }
    public Guid? ZoneId { get => CurrentZoneId; set => CurrentZoneId = value; }

    public UpdateAssetRequest() { }

    public UpdateAssetRequest(string name, string category, string status, Guid? currentZoneId = null)
    {
        Name = name;
        Category = category;
        Status = status;
        CurrentZoneId = currentZoneId;
    }
}

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

public class CreateIsolationPointRequest
{
    public Guid ZoneId { get; set; }
    public string? TagIdentifier { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? State { get; set; }
    public string? Notes { get; set; }

    public CreateIsolationPointRequest() { }

    public CreateIsolationPointRequest(Guid zoneId, string tagIdentifier, string description, string type, string state)
    {
        ZoneId = zoneId;
        TagIdentifier = tagIdentifier;
        Description = description;
        Type = type;
        State = state;
    }

    public string EffectiveTag => !string.IsNullOrWhiteSpace(TagIdentifier) ? TagIdentifier.Trim() 
        : (!string.IsNullOrWhiteSpace(Code) ? Code.Trim() : $"ISO-{DateTime.UtcNow.Ticks % 10000}");
    public string EffectiveDescription => !string.IsNullOrWhiteSpace(Description) ? Description.Trim() 
        : (!string.IsNullOrWhiteSpace(Name) ? Name.Trim() : "LOTO Isolation Valve/Breaker");
    public string EffectiveType => !string.IsNullOrWhiteSpace(Type) ? Type.Trim() : "Mechanical";
    public string EffectiveState => !string.IsNullOrWhiteSpace(State) ? State.Trim() : "Open";
}

public class UpdateIsolationPointStateRequest
{
    public string State { get; set; } = string.Empty;
    public string? CurrentState { get; set; }
    public string? LockedByUserId { get; set; }
    public string? Notes { get; set; }

    public UpdateIsolationPointStateRequest() { }

    public UpdateIsolationPointStateRequest(string state, string? lockedByUserId = null)
    {
        State = state;
        LockedByUserId = lockedByUserId;
    }

    public string EffectiveState => !string.IsNullOrWhiteSpace(State) ? State 
        : (!string.IsNullOrWhiteSpace(CurrentState) ? CurrentState : "Open");
}