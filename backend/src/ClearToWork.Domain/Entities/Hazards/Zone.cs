using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Hazards;

public class Zone : BaseAuditableEntity
{
    public Guid SiteId { get; set; }
    public Site? Site { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Medium";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusMeters { get; set; }
    public string? QrCodePayload { get; set; }
    public bool IsActive { get; set; } = true;
    public string GeoJsonGeometry { get; set; } = "{}";
    public ICollection<ZoneAdjacency> AdjacentZones { get; set; } = new List<ZoneAdjacency>();
}