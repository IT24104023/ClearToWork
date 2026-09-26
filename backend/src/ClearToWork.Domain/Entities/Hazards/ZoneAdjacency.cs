using System;

namespace ClearToWork.Domain.Entities.Hazards;

public class ZoneAdjacency
{
    public Guid ZoneId { get; set; }
    public Zone? Zone { get; set; }
    public Guid AdjacentZoneId { get; set; }
    public Zone? AdjacentZone { get; set; }
}
