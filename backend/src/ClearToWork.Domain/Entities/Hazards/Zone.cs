using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class Zone : BaseAuditableEntity
{
    public Guid SiteId { get; set; }
    public Site? Site { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Medium";
    public string GeoJsonGeometry { get; set; } = "{}";
    public ICollection<ZoneAdjacency> AdjacentZones { get; set; } = new List<ZoneAdjacency>();
}
