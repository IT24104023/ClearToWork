using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;

namespace ClearToWork.Domain.Entities.Hazards;

public class HazardType : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public HazardSeverity Severity { get; set; } = HazardSeverity.Medium;
    public ICollection<ControlMeasure> ControlMeasures { get; set; } = new List<ControlMeasure>();
}