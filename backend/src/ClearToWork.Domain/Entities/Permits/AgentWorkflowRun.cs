using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Permits;

public class AgentWorkflowRun : BaseAuditableEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public string OutcomeStatus { get; set; } = "PASSED";
    public long DurationMs { get; set; }
    public string ModelUsed { get; set; } = "gemini-2.5-pro";
    public string ExecutionTraceJson { get; set; } = "{}";
    public string? RecommendedFixJson { get; set; }
}
