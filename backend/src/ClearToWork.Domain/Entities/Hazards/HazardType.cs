using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Hazards;

public class HazardType : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public HazardSeverity SeverityLevel { get; set; }
    public double? MaxWindSpeedKmh { get; set; }
    public bool ProhibitedInRain { get; set; } = false;

    public ICollection<ControlMeasure> ControlMeasures { get; set; } = new List<ControlMeasure>();
}
