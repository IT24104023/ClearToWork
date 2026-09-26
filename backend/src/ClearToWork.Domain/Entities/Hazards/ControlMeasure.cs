using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Hazards;

public class ControlMeasure : BaseEntity
{
    public Guid HazardTypeId { get; set; }
    public HazardType? HazardType { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RequirementDescription { get => Description; set => Description = value; }
    public bool IsMandatory { get; set; } = true;
}