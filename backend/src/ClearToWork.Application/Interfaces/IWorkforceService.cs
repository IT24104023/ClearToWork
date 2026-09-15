using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IWorkforceService
{
    Task<List<WorkerDto>> GetAllWorkersAsync(string? trade = null, bool? activeOnly = true);
    Task<WorkerDto?> GetWorkerByIdAsync(Guid id);
    Task<WorkerDto> CreateWorkerAsync(string firstName, string lastName, string badgeNumber, string trade, Guid contractorId);
    Task<WorkerDto?> UpdateWorkerAsync(Guid id, UpdateWorkerRequest request);
    Task<bool> DeleteWorkerAsync(Guid id);
    Task<WorkerCertificateDto?> AddWorkerCertificateAsync(Guid workerId, CreateCertificateRequest request);
    Task<bool> DeleteWorkerCertificateAsync(Guid certificateId);
    Task<List<ContractorDto>> GetContractorsAsync();
    Task<List<CertificateTypeDto>> GetCertificateTypesAsync();
    Task<EligibilityCheckResponse> CheckEligibilityAsync(EligibilityCheckRequest request);
    Task<List<ExpiryForecastItem>> Get30DayExpiryForecastAsync();
}