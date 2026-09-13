using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _context;

    public EquipmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AssetDto>> GetAllAssetsAsync(string? category = null, string? status = null)
    {
        var query = _context.Assets
            .Include(a => a.CalibrationRecords)
            .Include(a => a.InspectionRecords)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<AssetCategory>(category, true, out var catEnum))
        {
            query = query.Where(a => a.Category == catEnum);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AssetStatus>(status, true, out var statEnum))
        {
            query = query.Where(a => a.Status == statEnum);
        }

        var assets = await query.ToListAsync();
        return assets.Select(MapAssetToDto).ToList();
    }

    public async Task<AssetDto?> GetAssetByIdAsync(Guid id)
    {
        var asset = await _context.Assets
            .Include(a => a.CalibrationRecords)
            .Include(a => a.InspectionRecords)
            .FirstOrDefaultAsync(a => a.Id == id);

        return asset == null ? null : MapAssetToDto(asset);
    }

    public async Task<EquipmentReadinessResponse> CheckReadinessAsync(EquipmentReadinessRequest request)
    {
        var assets = await _context.Assets
            .Include(a => a.CalibrationRecords)
            .Include(a => a.InspectionRecords)
            .Where(a => request.AssetIds.Contains(a.Id))
            .ToListAsync();

        var results = new List<AssetReadinessResult>();
        bool allReady = true;

        // Check overlapping reservations for the requested time window
        var overlappingReservations = await _context.PermitAssets
            .Include(pa => pa.PermitRequest)
            .Where(pa => request.AssetIds.Contains(pa.AssetId))
            .Where(pa => pa.PermitRequest!.Status == PermitStatus.Approved || pa.PermitRequest!.Status == PermitStatus.Active)
            .Where(pa => pa.ReservedFrom < request.EndTime && pa.ReservedUntil > request.StartTime)
            .Select(pa => pa.AssetId)
            .ToListAsync();

        foreach (var asset in assets)
        {
            var reasons = new List<string>();

            // 1. Status Check
            if (asset.Status == AssetStatus.OutOfService)
            {
                reasons.Add($"Asset {asset.AssetTag} is marked Out of Service.");
            }

            // 2. Reservation / Double-booking Check
            if (overlappingReservations.Contains(asset.Id))
            {
                reasons.Add($"Asset {asset.AssetTag} is already reserved in another active permit for this time window.");
            }

            // 3. Inspection Check
            var latestInspection = asset.InspectionRecords.OrderByDescending(i => i.InspectionDate).FirstOrDefault();
            if (latestInspection == null)
            {
                reasons.Add($"No inspection history found for {asset.AssetTag}.");
            }
            else if (!latestInspection.Passed || latestInspection.NextInspectionDate < request.StartTime)
            {
                int overdueDays = (request.StartTime.Date - latestInspection.NextInspectionDate.Date).Days;
                reasons.Add($"Asset {asset.AssetTag} inspection overdue by {Math.Max(1, overdueDays)} days (Inspection due {latestInspection.NextInspectionDate:yyyy-MM-dd}).");
            }

            // 4. Calibration Check (for Gas Detectors)
            if (asset.Category == AssetCategory.GasDetector)
            {
                var latestCalibration = asset.CalibrationRecords.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                if (latestCalibration == null || !latestCalibration.PassStatus || latestCalibration.NextCalibrationDate < request.StartTime)
                {
                    reasons.Add($"Gas detector {asset.AssetTag} calibration is expired or invalid.");
                }
            }

            bool isReady = reasons.Count == 0;
            if (!isReady) allReady = false;

            results.Add(new AssetReadinessResult(
                asset.Id,
                asset.AssetTag,
                asset.Name,
                isReady,
                reasons
            ));
        }

        // Recommend replacements for unready equipment
        var replacements = new List<AssetDto>();
        if (!allReady)
        {
            var neededCategories = assets
                .Where(a => results.Any(r => r.AssetId == a.Id && !r.IsReady))
                .Select(a => a.Category)
                .Distinct()
                .ToList();

            var validReplacements = await _context.Assets
                .Include(a => a.CalibrationRecords)
                .Include(a => a.InspectionRecords)
                .Where(a => !request.AssetIds.Contains(a.Id))
                .Where(a => neededCategories.Contains(a.Category))
                .Where(a => a.Status == AssetStatus.Available)
                .Where(a => a.InspectionRecords.Any(i => i.Passed && i.NextInspectionDate > request.EndTime))
                .Take(3)
                .ToListAsync();

            replacements = validReplacements.Select(MapAssetToDto).ToList();
        }

        return new EquipmentReadinessResponse(allReady, results, replacements);
    }

    public async Task<bool> ReserveEquipmentTransactionAsync(Guid permitId, List<Guid> assetIds, DateTime from, DateTime until)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Verify no double booking within transaction
            var hasOverlap = await _context.PermitAssets
                .Include(pa => pa.PermitRequest)
                .Where(pa => assetIds.Contains(pa.AssetId))
                .Where(pa => pa.PermitRequest!.Status == PermitStatus.Approved || pa.PermitRequest!.Status == PermitStatus.Active)
                .Where(pa => pa.ReservedFrom < until && pa.ReservedUntil > from)
                .AnyAsync();

            if (hasOverlap)
            {
                await transaction.RollbackAsync();
                return false;
            }

            foreach (var assetId in assetIds)
            {
                var asset = await _context.Assets.FindAsync(assetId);
                if (asset != null)
                {
                    asset.Status = AssetStatus.Reserved;
                }

                _context.PermitAssets.Add(new PermitAsset
                {
                    PermitRequestId = permitId,
                    AssetId = assetId,
                    ReservedFrom = from,
                    ReservedUntil = until
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<List<IsolationPointDto>> GetIsolationPointsForZoneAsync(Guid zoneId)
    {
        var points = await _context.IsolationPoints
            .Where(p => p.ZoneId == zoneId)
            .ToListAsync();

        return points.Select(p => new IsolationPointDto(
            p.Id,
            p.ZoneId,
            p.TagIdentifier,
            p.Description,
            p.Type.ToString(),
            p.State.ToString(),
            p.LockedByUserId,
            p.LockedAt
        )).ToList();
    }

    private static AssetDto MapAssetToDto(Asset a)
    {
        var latestCal = a.CalibrationRecords.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
        var latestInsp = a.InspectionRecords.OrderByDescending(i => i.InspectionDate).FirstOrDefault();

        bool calValid = latestCal != null && latestCal.PassStatus && latestCal.NextCalibrationDate > DateTime.UtcNow;
        bool inspValid = latestInsp != null && latestInsp.Passed && latestInsp.NextInspectionDate > DateTime.UtcNow;

        return new AssetDto(
            a.Id,
            a.AssetTag,
            a.Name,
            a.Category.ToString(),
            a.Status.ToString(),
            a.CurrentZoneId,
            calValid,
            latestCal?.NextCalibrationDate,
            inspValid,
            latestInsp?.NextInspectionDate
        );
    }
}
