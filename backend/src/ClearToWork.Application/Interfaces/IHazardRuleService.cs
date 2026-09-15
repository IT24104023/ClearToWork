using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IHazardRuleService
{
    // Zones & Analytics
    Task<List<ZoneDto>> GetAllZonesAsync();
    Task<ZoneDto?> GetZoneByCodeAsync(string zoneCode);
    Task<ZoneConflictCheckResponse> CheckZoneConflictsAsync(ZoneConflictCheckRequest request);
    Task<AnalyticsSafetySummaryDto> GetSafetyAnalyticsAsync();

    // Student 4: Observations CRUD
    Task<List<ObservationDto>> GetObservationsAsync(Guid? zoneId = null, string? category = null);
    Task<ObservationDto?> GetObservationByIdAsync(Guid id);
    Task<ObservationDto> CreateObservationAsync(Guid reportedByUserId, CreateObservationRequest request);
    Task<ObservationDto?> UpdateObservationAsync(Guid id, UpdateObservationRequest request);
    Task<bool> DeleteObservationAsync(Guid id);

    // Student 4: Rulebook & Hazard Types CRUD
    Task<List<HazardTypeDto>> GetHazardTypesAsync();
    Task<HazardTypeDto?> GetHazardTypeByIdAsync(Guid id);
    Task<HazardTypeDto> CreateHazardTypeAsync(CreateHazardTypeRequest request);
    Task<HazardTypeDto?> UpdateHazardTypeAsync(Guid id, UpdateHazardTypeRequest request);
    Task<bool> DeleteHazardTypeAsync(Guid id);

    // Student 4: Control Measures CRUD
    Task<ControlMeasureDto> CreateControlMeasureAsync(CreateControlMeasureRequest request);
    Task<ControlMeasureDto?> UpdateControlMeasureAsync(Guid id, UpdateControlMeasureRequest request);
    Task<bool> DeleteControlMeasureAsync(Guid id);

    // Student 4: SIMOPS Incompatibility Rules CRUD
    Task<List<IncompatibilityRuleDto>> GetIncompatibilityRulesAsync();
    Task<IncompatibilityRuleDto> CreateIncompatibilityRuleAsync(CreateIncompatibilityRuleRequest request);
    Task<IncompatibilityRuleDto?> UpdateIncompatibilityRuleAsync(Guid id, UpdateIncompatibilityRuleRequest request);
    Task<bool> DeleteIncompatibilityRuleAsync(Guid id);

    // Student 4: Zone Adjacencies CRUD
    Task<List<ZoneAdjacencyDto>> GetZoneAdjacenciesAsync();
    Task<bool> AddZoneAdjacencyAsync(AddZoneAdjacencyRequest request);
    Task<bool> RemoveZoneAdjacencyAsync(Guid zoneId, Guid adjacentZoneId);
}