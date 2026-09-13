using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IPermitLifecycleService
{
    Task<List<PermitDetailsDto>> GetPermitsAsync(string? status = null, Guid? contractorId = null, Guid? zoneId = null);
    Task<PermitDetailsDto?> GetPermitByIdAsync(Guid id);
    Task<PermitDetailsDto> CreatePermitDraftAsync(Guid supervisorId, CreatePermitRequest request);
    Task<ValidationReportDto> SubmitPermitForAiReviewAsync(Guid permitId);
    Task<PermitDetailsDto> RecordDecisionAsync(Guid permitId, Guid safetyOfficerId, PermitDecisionRequest request);
    Task<bool> ActivatePermitOnSiteAsync(Guid permitId, PermitActivationRequest request);
    Task<bool> CloseOutPermitAsync(Guid permitId, Guid userId, PermitCloseOutRequest request);
}
