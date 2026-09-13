using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class IncompatibilityRule : BaseEntity
{
    public string RuleCode { get; set; } = string.Empty;
    public Guid PrimaryHazardId { get; set; }
    public HazardType? PrimaryHazard { get; set; }
    public Guid ConflictingHazardId { get; set; }
    public HazardType? ConflictingHazard { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool AppliesToAdjacentZones { get; set; } = true;
}
