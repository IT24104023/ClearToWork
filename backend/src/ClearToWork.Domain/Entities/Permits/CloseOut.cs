using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Permits;

public class CloseOut : BaseAuditableEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public Guid ClosedByUserId { get; set; }
    public bool SiteCleaned { get; set; } = true;
    public bool ToolsRemoved { get; set; } = true;
    public bool IsolationsRestored { get; set; } = true;
    public DateTime SignOffTimestamp { get; set; } = DateTime.UtcNow;
    public string FinalComments { get; set; } = string.Empty;
}
