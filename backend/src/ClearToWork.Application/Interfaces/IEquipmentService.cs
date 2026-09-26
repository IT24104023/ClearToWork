using ClearToWork.Application.DTOs;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Application.Interfaces;

public interface IEquipmentService
{
    Task<List<AssetDto>> GetAllAssetsAsync(string? category = null, string? status = null);
    Task<AssetDto?> GetAssetByIdAsync(Guid id);
    Task<AssetDto> CreateAssetAsync(CreateAssetRequest request);
    Task<AssetDto?> UpdateAssetAsync(Guid id, UpdateAssetRequest request);
    Task<bool> DeleteAssetAsync(Guid id);
    Task<bool> AddInspectionRecordAsync(Guid assetId, CreateInspectionRequest request);
    Task<bool> AddCalibrationRecordAsync(Guid assetId, CreateCalibrationRequest request);
    Task<EquipmentReadinessResponse> CheckReadinessAsync(EquipmentReadinessRequest request);
    Task<bool> ReserveEquipmentTransactionAsync(Guid permitId, List<Guid> assetIds, DateTime from, DateTime until);
    Task<List<IsolationPointDto>> GetIsolationPointsForZoneAsync(Guid zoneId);
    Task<IsolationPointDto> CreateIsolationPointAsync(CreateIsolationPointRequest request);
    Task<IsolationPointDto?> UpdateIsolationPointStateAsync(Guid id, UpdateIsolationPointStateRequest request);
}