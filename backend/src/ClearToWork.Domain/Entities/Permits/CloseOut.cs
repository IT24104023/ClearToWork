using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Permits;

public class CloseOut : BaseEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public bool SiteCleaned { get; set; }
    public bool ToolsRemoved { get; set; }
    public bool IsolationsRestored { get; set; }
    public string FinalComments { get; set; } = string.Empty;
    public DateTime ClosedAt { get; set; } = DateTime.UtcNow;
    public string ClosedBy { get; set; } = string.Empty;
}
