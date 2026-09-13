using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class Site : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public ICollection<Zone> Zones { get; set; } = new List<Zone>();
}
