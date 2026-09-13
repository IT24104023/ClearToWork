using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class Zone : BaseAuditableEntity
{
    public Guid SiteId { get; set; }
    public Site? Site { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public double RadiusMeters { get; set; } = 50.0;
    public string QrCodePayload { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ZoneAdjacency> AdjacentZones { get; set; } = new List<ZoneAdjacency>();
}
