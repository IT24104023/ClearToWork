using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class ControlMeasure : BaseEntity
{
    public Guid HazardTypeId { get; set; }
    public HazardType? HazardType { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
}
