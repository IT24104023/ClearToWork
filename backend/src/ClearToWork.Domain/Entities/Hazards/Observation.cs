using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class Observation : BaseAuditableEntity
{
    public Guid ZoneId { get; set; }
    public Guid ReportedByUserId { get; set; }
    public string Category { get; set; } = "NearMiss";
    public string Description { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}
