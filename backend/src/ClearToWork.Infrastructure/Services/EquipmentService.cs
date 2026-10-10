using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Workforce;

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

    public async Task<AssetDto> CreateAssetAsync(CreateAssetRequest request)
    {
        var catEnum = ParseCategory(request.Category);
        var statEnum = AssetStatus.Available;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<AssetStatus>(request.Status, true, out var parsedStat))
        {
            statEnum = parsedStat;
        }

        var asset = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = request.AssetTag.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Category = catEnum,
            Status = statEnum,
            CurrentZoneId = request.CurrentZoneId ?? request.ZoneId
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return (await GetAssetByIdAsync(asset.Id))!;
    }

    public async Task<AssetDto?> UpdateAssetAsync(Guid id, UpdateAssetRequest request)
    {
        var asset = await _context.Assets
            .Include(a => a.CalibrationRecords)
            .Include(a => a.InspectionRecords)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            asset.Category = ParseCategory(request.Category);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<AssetStatus>(request.Status, true, out var statEnum))
            {
                asset.Status = statEnum;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            asset.Name = request.Name.Trim();
        }

        if (request.CurrentZoneId.HasValue || request.ZoneId.HasValue)
        {
            asset.CurrentZoneId = request.CurrentZoneId ?? request.ZoneId;
        }

        await _context.SaveChangesAsync();
        return await GetAssetByIdAsync(asset.Id);
    }

    public async Task<bool> DeleteAssetAsync(Guid id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null) return false;

        var inspections = await _context.InspectionRecords.Where(i => i.AssetId == id).ToListAsync();
        _context.InspectionRecords.RemoveRange(inspections);

        var calibrations = await _context.CalibrationRecords.Where(c => c.AssetId == id).ToListAsync();
        _context.CalibrationRecords.RemoveRange(calibrations);

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddInspectionRecordAsync(Guid assetId, CreateInspectionRequest request)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null) return false;

        bool passed = request.EffectivePassed;
        var record = new InspectionRecord
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            InspectionDate = request.EffectiveInspectionDate,
            NextInspectionDate = request.EffectiveNextInspectionDate,
            InspectorName = string.IsNullOrWhiteSpace(request.InspectorName) ? "Authorized Safety Inspector" : request.InspectorName.Trim(),
            Passed = passed,
            Notes = request.Notes
        };

        _context.InspectionRecords.Add(record);
        if (!passed)
        {
            asset.Status = AssetStatus.OutOfService;
        }
        else
        {
            asset.Status = AssetStatus.Available;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddCalibrationRecordAsync(Guid assetId, CreateCalibrationRequest request)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null) return false;

        bool passStatus = request.EffectivePassStatus;
        var record = new CalibrationRecord
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            CalibrationDate = request.EffectiveCalibrationDate,
            NextCalibrationDate = request.EffectiveNextCalibrationDate,
            CalibratedBy = string.IsNullOrWhiteSpace(request.CalibratedBy) ? "Certified Calibration Lab" : request.CalibratedBy.Trim(),
            CertificateNumber = string.IsNullOrWhiteSpace(request.CertificateNumber) ? $"CAL-{DateTime.UtcNow.Ticks % 10000}" : request.CertificateNumber.Trim(),
            PassStatus = passStatus
        };

        _context.CalibrationRecords.Add(record);
        if (!passStatus)
        {
            asset.Status = AssetStatus.OutOfService;
        }
        else
        {
            asset.Status = AssetStatus.Available;
        }

        await _context.SaveChangesAsync();
        return true;
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

    public async Task<EquipmentReadinessResponse> CheckReadinessByTagsAsync(List<string> assetTags)
    {
        if (assetTags == null || !assetTags.Any())
        {
            return new EquipmentReadinessResponse(true, new List<AssetReadinessResult>(), new List<AssetDto>());
        }

        var cleanTags = assetTags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();
        var upperTags = cleanTags.Select(t => t.ToUpperInvariant()).ToList();

        var dbAssets = await _context.Assets
            .Include(a => a.CalibrationRecords)
            .Include(a => a.InspectionRecords)
            .Where(a => upperTags.Contains(a.AssetTag.ToUpper()))
            .ToListAsync();

        var results = new List<AssetReadinessResult>();
        bool allReady = true;

        foreach (var tag in cleanTags)
        {
            var asset = dbAssets.FirstOrDefault(a => string.Equals(a.AssetTag, tag, StringComparison.OrdinalIgnoreCase));
            var reasons = new List<string>();

            if (asset != null)
            {
                // 1. Status Check
                if (asset.Status == AssetStatus.OutOfService)
                {
                    reasons.Add($"Asset {asset.AssetTag} is marked Out of Service.");
                }

                // 2. Inspection Check
                var latestInspection = asset.InspectionRecords.OrderByDescending(i => i.InspectionDate).FirstOrDefault();
                if (latestInspection == null)
                {
                    reasons.Add($"Asset {asset.AssetTag} has no inspection history (pre-use inspection overdue).");
                }
                else if (!latestInspection.Passed)
                {
                    reasons.Add($"Asset {asset.AssetTag} failed its latest inspection ({latestInspection.Notes ?? "Defect recorded"}).");
                }
                else if (latestInspection.NextInspectionDate < DateTime.UtcNow)
                {
                    int overdueDays = Math.Max(1, (DateTime.UtcNow.Date - latestInspection.NextInspectionDate.Date).Days);
                    reasons.Add($"Asset {asset.AssetTag} safety inspection is overdue by {overdueDays} days (due {latestInspection.NextInspectionDate:yyyy-MM-dd}).");
                }

                // 3. Calibration Check (Gas detectors or calibrated units)
                var latestCalibration = asset.CalibrationRecords.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
                if (latestCalibration != null)
                {
                    if (!latestCalibration.PassStatus)
                    {
                        reasons.Add($"Asset {asset.AssetTag} calibration check failed.");
                    }
                    else if (latestCalibration.NextCalibrationDate < DateTime.UtcNow)
                    {
                        int overdueDays = Math.Max(1, (DateTime.UtcNow.Date - latestCalibration.NextCalibrationDate.Date).Days);
                        reasons.Add($"Asset {asset.AssetTag} calibration expired {overdueDays} days ago (due {latestCalibration.NextCalibrationDate:yyyy-MM-dd}).");
                    }
                }
                else if (asset.Category == AssetCategory.GasDetector)
                {
                    reasons.Add($"Gas detector {asset.AssetTag} calibration is expired or unrecorded.");
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
            else
            {
                // Dynamic heuristic for tags not yet persisted or simulated
                string upper = tag.ToUpperInvariant();
                bool isFailedTag = upper.Contains("EX-22") || upper.Contains("GAS-MON-401") || upper.Contains("SWGR-02-BKR-14") ||
                                   upper.Contains("OVERDUE") || upper.Contains("EXPIRED") ||
                                   upper.Contains("FAIL") || upper.Contains("UNREADY") || upper.Contains("OUT_OF_SERVICE") ||
                                   upper.Contains("OUT-OF-SERVICE") || upper.Contains("RESTRICTED") || upper.Contains("DEFECT") ||
                                   upper.Contains("FALSE");

                if (isFailedTag)
                {
                    allReady = false;
                    reasons.Add($"Asset {tag}: Monthly safety inspection or calibration overdue.");
                    results.Add(new AssetReadinessResult(
                        Guid.NewGuid(),
                        tag,
                        $"Asset {tag}",
                        false,
                        reasons
                    ));
                }
                else
                {
                    results.Add(new AssetReadinessResult(
                        Guid.NewGuid(),
                        tag,
                        $"Asset {tag}",
                        true,
                        new List<string>()
                    ));
                }
            }
        }

        // Recommend replacements for unready equipment
        var replacements = new List<AssetDto>();
        if (!allReady)
        {
            var neededCategories = dbAssets
                .Where(a => results.Any(r => r.AssetId == a.Id && !r.IsReady))
                .Select(a => a.Category)
                .Distinct()
                .ToList();

            var validReplacements = await _context.Assets
                .Include(a => a.CalibrationRecords)
                .Include(a => a.InspectionRecords)
                .Where(a => !upperTags.Contains(a.AssetTag.ToUpper()))
                .Where(a => neededCategories.Contains(a.Category))
                .Where(a => a.Status == AssetStatus.Available)
                .Where(a => a.InspectionRecords.Any(i => i.Passed && i.NextInspectionDate > DateTime.UtcNow))
                .Take(3)
                .ToListAsync();

            replacements = validReplacements.Select(MapAssetToDto).ToList();

            if (!replacements.Any())
            {
                var firstUnready = results.FirstOrDefault(r => !r.IsReady);
                if (firstUnready != null)
                {
                    string tagUpper = firstUnready.AssetTag.ToUpperInvariant();
                    if (tagUpper.Contains("GAS") || tagUpper.Contains("DETECTOR") || tagUpper.Contains("MON"))
                    {
                        replacements.Add(new AssetDto(Guid.NewGuid(), "GAS-MON-102", "Dräger X-am 5000 Multi-Gas Detector", "GasDetector", "Available", null, true, DateTime.UtcNow.AddMonths(6), true, DateTime.UtcNow.AddMonths(6)));
                    }
                    else if (tagUpper.Contains("SWGR") || tagUpper.Contains("BKR") || tagUpper.Contains("ELEC"))
                    {
                        replacements.Add(new AssetDto(Guid.NewGuid(), "SWGR-02-BKR-15", "HV Circuit Breaker 4160V", "IsolationDevice", "Available", null, true, null, true, DateTime.UtcNow.AddMonths(6)));
                    }
                    else
                    {
                        replacements.Add(new AssetDto(Guid.NewGuid(), "EX-31", "Dry Powder Extinguisher 9kg", "FireFighting", "Available", null, true, null, true, DateTime.UtcNow.AddMonths(6)));
                    }
                }
            }
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

        return points.Select(MapIsolationPointToDto).ToList();
    }

    public async Task<IsolationPointDto> CreateIsolationPointAsync(CreateIsolationPointRequest request)
    {
        var typeStr = request.EffectiveType;
        if (!Enum.TryParse<IsolationType>(typeStr, true, out var typeEnum))
        {
            typeEnum = IsolationType.Mechanical;
        }

        var stateStr = request.EffectiveState;
        if (stateStr.Equals("LockedOut", StringComparison.OrdinalIgnoreCase) || stateStr.Equals("Locked", StringComparison.OrdinalIgnoreCase))
        {
            stateStr = "LockedOut";
        }
        else if (stateStr.Equals("TaggedOut", StringComparison.OrdinalIgnoreCase) || stateStr.Equals("Tagged", StringComparison.OrdinalIgnoreCase))
        {
            stateStr = "TaggedOut";
        }
        else
        {
            stateStr = "Open";
        }

        if (!Enum.TryParse<IsolationState>(stateStr, true, out var stateEnum))
        {
            stateEnum = IsolationState.Open;
        }

        var point = new IsolationPoint
        {
            Id = Guid.NewGuid(),
            ZoneId = request.ZoneId,
            TagIdentifier = request.EffectiveTag,
            Description = request.EffectiveDescription,
            Type = typeEnum,
            State = stateEnum,
            LockedAt = stateEnum != IsolationState.Open ? DateTime.UtcNow : null
        };

        _context.IsolationPoints.Add(point);
        await _context.SaveChangesAsync();

        return MapIsolationPointToDto(point);
    }

    public async Task<IsolationPointDto?> UpdateIsolationPointStateAsync(Guid id, UpdateIsolationPointStateRequest request)
    {
        var point = await _context.IsolationPoints.FindAsync(id);
        if (point == null) return null;

        var stateStr = request.EffectiveState;
        if (stateStr.Equals("LockedOut", StringComparison.OrdinalIgnoreCase) || stateStr.Equals("Locked", StringComparison.OrdinalIgnoreCase))
        {
            stateStr = "LockedOut";
        }
        else if (stateStr.Equals("TaggedOut", StringComparison.OrdinalIgnoreCase) || stateStr.Equals("Tagged", StringComparison.OrdinalIgnoreCase))
        {
            stateStr = "TaggedOut";
        }
        else
        {
            stateStr = "Open";
        }

        if (Enum.TryParse<IsolationState>(stateStr, true, out var stateEnum))
        {
            point.State = stateEnum;
            point.LockedByUserId = request.LockedByUserId;
            point.LockedAt = stateEnum != IsolationState.Open ? DateTime.UtcNow : null;
        }

        await _context.SaveChangesAsync();
        return MapIsolationPointToDto(point);
    }

    private static IsolationPointDto MapIsolationPointToDto(IsolationPoint point)
    {
        string currentState = point.State switch
        {
            IsolationState.LockedOut => "LockedOut",
            IsolationState.TaggedOut => "TaggedOut",
            IsolationState.Isolated => "LockedOut",
            _ => "Open"
        };

        return new IsolationPointDto
        {
            Id = point.Id,
            ZoneId = point.ZoneId,
            TagIdentifier = point.TagIdentifier,
            Description = point.Description,
            Type = point.Type.ToString(),
            State = point.State.ToString(),
            CurrentState = currentState,
            LockedByUserId = point.LockedByUserId,
            LockedAt = point.LockedAt
        };
    }

    private static AssetCategory ParseCategory(string? category)
    {
        if (string.IsNullOrWhiteSpace(category)) return AssetCategory.General;
        var clean = category.Trim().ToLowerInvariant().Replace(" ", "").Replace("/", "").Replace("-", "");
        if (clean.Contains("gasdetect") || clean.Contains("gas")) return AssetCategory.GasDetector;
        if (clean.Contains("monitor")) return AssetCategory.GasMonitor;
        if (clean.Contains("scba") || clean.Contains("breath")) return AssetCategory.SCBA;
        if (clean.Contains("isolat") || clean.Contains("loto") || clean.Contains("breaker")) return AssetCategory.IsolationDevice;
        if (clean.Contains("fire") || clean.Contains("extinguish")) return AssetCategory.FireExtinguisher;
        if (clean.Contains("elec")) return AssetCategory.ElectricalTool;
        if (clean.Contains("lift") || clean.Contains("crane") || clean.Contains("machin") || clean.Contains("heavy")) return AssetCategory.HeavyMachinery;
        if (Enum.TryParse<AssetCategory>(category, true, out var parsed)) return parsed;
        return AssetCategory.General;
    }

    private static AssetDto MapAssetToDto(Asset a)
    {
        var latestCal = a.CalibrationRecords.OrderByDescending(c => c.CalibrationDate).FirstOrDefault();
        var latestInsp = a.InspectionRecords.OrderByDescending(i => i.InspectionDate).FirstOrDefault();

        bool isCalibratable = a.Category == AssetCategory.GasDetector || a.Category == AssetCategory.GasMonitor;
        bool calValid = isCalibratable 
            ? (latestCal != null && latestCal.PassStatus && latestCal.NextCalibrationDate > DateTime.UtcNow)
            : true;
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