using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitRequest : BaseAuditableEntity
{
    public string PermitNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ObjectiveDescription { get; set; } = string.Empty;
    public Guid PermitTypeId { get; set; }
    public PermitType? PermitType { get; set; }
    public PermitStatus Status { get; set; } = PermitStatus.Draft;
    public string ZoneCode { get; set; } = "ZONE-A1";
    public Guid ZoneId { get; set; }
    public string IssuingAuthority { get; set; } = "Mohammed Zakee";
    public string SupervisorName { get; set; } = "Mohammed Zakee";
    public string? PermitQrToken { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime ScheduledStartTime { get; set; } = DateTime.UtcNow;
    public DateTime ScheduledEndTime { get; set; } = DateTime.UtcNow.AddHours(12);
    public ICollection<PermitWorker> AssignedWorkers { get; set; } = new List<PermitWorker>();
    public ICollection<PermitAsset> AssignedAssets { get; set; } = new List<PermitAsset>();
    public ICollection<EvidencePhoto> Photos { get; set; } = new List<EvidencePhoto>();
    public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    public CloseOut? CloseOut { get; set; }
    public AgentWorkflowRun? AgentWorkflowRun { get; set; }
}
