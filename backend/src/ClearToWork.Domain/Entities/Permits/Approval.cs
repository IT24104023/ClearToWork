using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Permits;

public class Approval : BaseAuditableEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public Guid SafetyOfficerId { get; set; }
    public DecisionType Decision { get; set; }
    public string DecisionNotes { get; set; } = string.Empty;
    public DateTime DecisionTimestamp { get; set; } = DateTime.UtcNow;
}
