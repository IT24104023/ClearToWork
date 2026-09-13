using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IHazardRuleService
{
    Task<List<ZoneDto>> GetAllZonesAsync();
    Task<ZoneDto?> GetZoneByCodeAsync(string zoneCode);
    Task<ZoneConflictCheckResponse> CheckZoneConflictsAsync(ZoneConflictCheckRequest request);
    Task<AnalyticsSafetySummaryDto> GetSafetyAnalyticsAsync();
}
