using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Hazards;

public class ZoneAdjacency
{
    public Guid ZoneId { get; set; }
    public Zone? Zone { get; set; }
    public Guid AdjacentZoneId { get; set; }
    public Zone? AdjacentZone { get; set; }
}
