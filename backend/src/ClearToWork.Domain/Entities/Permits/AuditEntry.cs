using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Permits;

public class AuditEntry : BaseEntity
{
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid PerformedByUserId { get; set; }
    public string ChangesJson { get; set; } = "{}";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
