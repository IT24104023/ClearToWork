using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Permits;

public class AgentWorkflowRun : BaseAuditableEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public WorkflowOutcome OutcomeStatus { get; set; }
    public long DurationMs { get; set; }
    public string ModelUsed { get; set; } = "mistral:7b";
    public string ExecutionTraceJson { get; set; } = "{}";
    public string RecommendedFixJson { get; set; } = "{}";
}
