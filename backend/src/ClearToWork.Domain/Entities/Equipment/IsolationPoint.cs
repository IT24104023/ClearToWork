using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Equipment;

public class IsolationPoint : BaseAuditableEntity
{
    public Guid ZoneId { get; set; }
    public string TagIdentifier { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IsolationType Type { get; set; }
    public IsolationState State { get; set; } = IsolationState.Open;
    public string? LockedByUserId { get; set; }
    public DateTime? LockedAt { get; set; }
}
