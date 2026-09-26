using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

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
    public string? ClosedByUserId { get => ClosedBy; set => ClosedBy = value ?? string.Empty; }
    public DateTime? SignOffTimestamp { get => ClosedAt; set => ClosedAt = value ?? DateTime.UtcNow; }
}