using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class ZoneAdjacency : BaseEntity
{
    public Guid ZoneId { get; set; }
    public Zone? Zone { get; set; }
    public Guid AdjacentZoneId { get; set; }
    public Zone? AdjacentZone { get; set; }
    public string RiskTransferLevel { get; set; } = "Medium";
}
