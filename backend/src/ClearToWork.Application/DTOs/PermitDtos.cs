using ClearToWork.Domain.Enums;

namespace ClearToWork.Application.DTOs;

public record CreatePermitRequest(
    Guid PermitTypeId,
    Guid ZoneId,
    string ObjectiveDescription,
    DateTime ScheduledStartTime,
    DateTime ScheduledEndTime,
    List<Guid> WorkerIds,
    List<Guid> AssetIds,
    List<string> PhotoUrls
);

public record UpdatePermitRequest(
    Guid PermitTypeId,
    Guid ZoneId,
    string ObjectiveDescription,
    DateTime ScheduledStartTime,
    DateTime ScheduledEndTime,
    List<Guid> WorkerIds,
    List<Guid> AssetIds,
    List<string>? PhotoUrls = null
);

public record PermitDetailsDto(
    Guid Id,
    string PermitNumber,
    string PermitTypeName,
    string PermitTypeCode,
    string ZoneName,
    string ZoneCode,
    string SupervisorName,
    string ObjectiveDescription,
    DateTime ScheduledStartTime,
    DateTime ScheduledEndTime,
    string Status,
    string? PermitQrToken,
    DateTime? ActivatedAt,
    List<WorkerDto> AssignedWorkers,
    List<AssetDto> AssignedAssets,
    List<EvidencePhotoDto> Photos,
    ApprovalDto? Approval,
    AgentWorkflowRunDto? WorkflowRun
);

public record EvidencePhotoDto(
    Guid Id,
    string Stage,
    string PhotoUrl,
    decimal? GpsLatitude,
    decimal? GpsLongitude,
    DateTime CapturedAt
);

public record ApprovalDto(
    string SafetyOfficerName,
    string Decision,
    string DecisionNotes,
    DateTime DecisionTimestamp
);

public record AgentWorkflowRunDto(
    Guid Id,
    string OutcomeStatus,
    long DurationMs,
    string ModelUsed,
    string ExecutionTraceJson,
    string? RecommendedFixJson,
    DateTime CreatedAt
);

public record PermitDecisionRequest(
    DecisionType Decision,
    string DecisionNotes
);

public record PermitActivationRequest(
    string ScannedQrPayload,
    decimal CurrentLatitude,
    decimal CurrentLongitude
);

public record PermitCloseOutRequest(
    bool SiteCleaned,
    bool ToolsRemoved,
    bool IsolationsRestored,
    string FinalComments,
    List<string> CloseOutPhotoUrls
);

public record ValidationReportDto(
    bool IsApproved,
    string Verdict,
    List<string> HardFailureReasons,
    List<string> WarningNotes,
    AgentProposedFixDto? ProposedFix
);

public record AgentProposedFixDto(
    string SuggestedWorkerBadge,
    string SuggestedAssetTag,
    string SuggestedTimeWindow,
    string SummaryExplanation
);
