using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Hazards;

public class IncompatibilityRule : BaseAuditableEntity
{
    public Guid PrimaryPermitTypeId { get; set; }
    public Guid SecondaryPermitTypeId { get; set; }
    public string RestrictionLevel { get; set; } = "Prohibited";
    public double BufferDistanceMeters { get; set; } = 50.0;
    public string ExplanationReason { get; set; } = string.Empty;
}
