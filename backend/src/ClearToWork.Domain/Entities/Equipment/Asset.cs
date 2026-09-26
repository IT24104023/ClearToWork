using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;
using ClearToWork.Domain.Entities.Permits;

namespace ClearToWork.Domain.Entities.Equipment;

public class Asset : BaseAuditableEntity
{
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AssetCategory Category { get; set; }
    public AssetStatus Status { get; set; } = AssetStatus.Available;
    public Guid? CurrentZoneId { get; set; }
    public ICollection<CalibrationRecord> CalibrationRecords { get; set; } = new List<CalibrationRecord>();
    public ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();
    public ICollection<PermitAsset> PermitAssignments { get; set; } = new List<PermitAsset>();
}
