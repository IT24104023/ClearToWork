using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class Observation : BaseAuditableEntity
{
    public Guid ZoneId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string ReportedBy { get; set; } = string.Empty;
    public DateTime ObservedAt { get; set; } = DateTime.UtcNow;
}
