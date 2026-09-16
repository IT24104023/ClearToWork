using ClearToWork.Application.DTOs;

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