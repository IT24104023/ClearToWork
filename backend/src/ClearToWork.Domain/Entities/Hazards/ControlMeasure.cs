using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class ControlMeasure : BaseEntity
{
    public Guid HazardTypeId { get; set; }
    public HazardType? HazardType { get; set; }
    public string Code { get; set; } = string.Empty;
    public string RequirementDescription { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
}
