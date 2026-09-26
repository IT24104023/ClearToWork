using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class IncompatibilityRule : BaseAuditableEntity
{
    public Guid PrimaryPermitTypeId { get; set; }
    public Guid SecondaryPermitTypeId { get; set; }
    public string RestrictionLevel { get; set; } = "Prohibited";
    public double BufferDistanceMeters { get; set; } = 50.0;
    public string ExplanationReason { get; set; } = string.Empty;
}
