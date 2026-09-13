using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitRequest : BaseAuditableEntity
{
    public string PermitNumber { get; set; } = string.Empty;
    public Guid PermitTypeId { get; set; }
    public PermitType? PermitType { get; set; }
    public Guid ZoneId { get; set; }
    public Guid SupervisorId { get; set; }
    public string ObjectiveDescription { get; set; } = string.Empty;
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public PermitStatus Status { get; set; } = PermitStatus.Draft;
    public string? PermitQrToken { get; set; }
    public decimal? ActivationGpsLatitude { get; set; }
    public decimal? ActivationGpsLongitude { get; set; }
    public DateTime? ActivatedAt { get; set; }

    public ICollection<PermitWorker> AssignedWorkers { get; set; } = new List<PermitWorker>();
    public ICollection<PermitAsset> AssignedAssets { get; set; } = new List<PermitAsset>();
    public ICollection<EvidencePhoto> EvidencePhotos { get; set; } = new List<EvidencePhoto>();
    public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    public CloseOut? CloseOut { get; set; }
    public AgentWorkflowRun? AgentWorkflowRun { get; set; }
}
