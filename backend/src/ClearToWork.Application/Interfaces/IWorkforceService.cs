using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IWorkforceService
{
    Task<List<WorkerDto>> GetAllWorkersAsync(string? trade = null, bool? activeOnly = true);
    Task<WorkerDto?> GetWorkerByIdAsync(Guid id);
    Task<WorkerDto> CreateWorkerAsync(string firstName, string lastName, string badgeNumber, string trade, Guid contractorId);
    Task<EligibilityCheckResponse> CheckEligibilityAsync(EligibilityCheckRequest request);
    Task<List<ExpiryForecastItem>> Get30DayExpiryForecastAsync();
}
