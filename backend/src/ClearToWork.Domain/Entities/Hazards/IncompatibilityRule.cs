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
    public string RuleCode { get; set; } = string.Empty;
    public Guid PrimaryPermitTypeId { get; set; }
    public Guid SecondaryPermitTypeId { get; set; }
    public Guid PrimaryHazardId { get; set; }
    public HazardType? PrimaryHazard { get; set; }
    public Guid ConflictingHazardId { get; set; }
    public HazardType? ConflictingHazard { get; set; }
    public string RestrictionLevel { get; set; } = "Prohibited";
    public double BufferDistanceMeters { get; set; } = 50.0;
    public string ExplanationReason { get; set; } = string.Empty;
    public string Reason { get => ExplanationReason; set => ExplanationReason = value; }
    public bool AppliesToAdjacentZones { get; set; } = true;
}