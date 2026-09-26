using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Identity;

public class AuditEntry : BaseEntity
{
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string ChangesJson { get; set; } = "{}";
}
