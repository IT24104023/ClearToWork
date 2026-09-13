using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IEquipmentService
{
    Task<List<AssetDto>> GetAllAssetsAsync(string? category = null, string? status = null);
    Task<AssetDto?> GetAssetByIdAsync(Guid id);
    Task<EquipmentReadinessResponse> CheckReadinessAsync(EquipmentReadinessRequest request);
    Task<bool> ReserveEquipmentTransactionAsync(Guid permitId, List<Guid> assetIds, DateTime from, DateTime until);
    Task<List<IsolationPointDto>> GetIsolationPointsForZoneAsync(Guid zoneId);
}
