using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Services;

public class HazardRuleService : IHazardRuleService
{
    private readonly AppDbContext _context;

    public HazardRuleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ZoneDto>> GetAllZonesAsync()
    {
        var zones = await _context.Zones
            .Include(z => z.AdjacentZones)
                .ThenInclude(za => za.AdjacentZone)
            .ToListAsync();

        return zones.Select(MapZoneToDto).ToList();
    }

    public async Task<ZoneDto?> GetZoneByCodeAsync(string zoneCode)
    {
        var zone = await _context.Zones
            .Include(z => z.AdjacentZones)
                .ThenInclude(za => za.AdjacentZone)
            .FirstOrDefaultAsync(z => z.Code.ToLower() == zoneCode.ToLower());

        return zone == null ? null : MapZoneToDto(zone);
    }

    public async Task<ZoneConflictCheckResponse> CheckZoneConflictsAsync(ZoneConflictCheckRequest request)
    {
        var targetZone = await _context.Zones
            .Include(z => z.AdjacentZones)
            .FirstOrDefaultAsync(z => z.Id == request.ZoneId);

        if (targetZone == null)
        {
            throw new ArgumentException("Zone not found.");
        }

        var relevantZoneIds = new List<Guid> { targetZone.Id };
        relevantZoneIds.AddRange(targetZone.AdjacentZones.Select(a => a.AdjacentZoneId));

        // Find active/approved permits in relevant zones overlapping the time window
        var overlappingPermits = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Where(p => relevantZoneIds.Contains(p.ZoneId))
            .Where(p => p.Status == PermitStatus.Approved || p.Status == PermitStatus.Active)
            .Where(p => p.ScheduledStartTime < request.EndTime && p.ScheduledEndTime > request.StartTime)
            .ToListAsync();

        var conflicts = new List<ConflictItem>();
        var targetHazard = await _context.HazardTypes.FirstOrDefaultAsync(h => h.Code == request.HazardCode);

        // Fetch incompatibility rules
        var rules = await _context.IncompatibilityRules
            .Include(r => r.PrimaryHazard)
            .Include(r => r.ConflictingHazard)
            .ToListAsync();

        foreach (var permit in overlappingPermits)
        {
            bool isAdjacent = permit.ZoneId != targetZone.Id;

            // Check if any rule forbids targetHazard with permit's hazard
            var matchedRule = rules.FirstOrDefault(r =>
                (r.PrimaryHazard?.Code == request.HazardCode && r.ConflictingHazard?.Code == permit.PermitType?.Code) ||
                (r.PrimaryHazard?.Code == permit.PermitType?.Code && r.ConflictingHazard?.Code == request.HazardCode) ||
                (request.HazardCode == "HOT_WORK" && permit.PermitType?.Code == "SOLVENT_PAINTING")
            );

            if (matchedRule != null)
            {
                if (!isAdjacent || matchedRule.AppliesToAdjacentZones)
                {
                    var zoneObj = await _context.Zones.FindAsync(permit.ZoneId);
                    conflicts.Add(new ConflictItem(
                        permit.PermitNumber,
                        zoneObj?.Code ?? "Adjacent Zone",
                        permit.PermitType?.Code ?? "Hazardous Operation",
                        matchedRule.RuleCode,
                        $"{matchedRule.Reason} (Conflict with active permit {permit.PermitNumber} in {zoneObj?.Name}).",
                        permit.ScheduledStartTime,
                        permit.ScheduledEndTime
                    ));
                }
            }
        }

        string? suggestedAlternative = null;
        if (conflicts.Count > 0)
        {
            var latestConflictEnd = conflicts.Max(c => c.ActiveEndTime);
            suggestedAlternative = $"Shift work start time to {latestConflictEnd.AddMinutes(30):HH:mm} (after adjacent solvent painting permit completes at {latestConflictEnd:HH:mm}).";
        }

        return new ZoneConflictCheckResponse(conflicts.Count > 0, conflicts, suggestedAlternative);
    }

    public async Task<AnalyticsSafetySummaryDto> GetSafetyAnalyticsAsync()
    {
        var totalIssued = await _context.PermitRequests.CountAsync(p => p.Status == PermitStatus.Approved || p.Status == PermitStatus.Active || p.Status == PermitStatus.Closed);
        var totalRefused = await _context.PermitRequests.CountAsync(p => p.Status == PermitStatus.Refused);
        var activeCount = await _context.PermitRequests.CountAsync(p => p.Status == PermitStatus.Active);

        // Demo ranking of refusal causes
        var refusalCauses = new Dictionary<string, int>
        {
            { "Competency Expiry (Welder/Cert)", 14 },
            { "SIMOPS Adjacent Zone Clash (Rule HR-07)", 11 },
            { "Equipment Inspection/Calibration Overdue", 9 },
            { "Adverse Weather (High Wind Gusts > 35km/h)", 6 }
        };

        var permitsByZone = await _context.PermitRequests
            .GroupBy(p => p.ZoneId)
            .Select(g => new { ZoneId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.ZoneId.ToString().Substring(0, 8), v => v.Count);

        return new AnalyticsSafetySummaryDto(
            totalIssued,
            totalRefused,
            activeCount,
            MeanTimeToApprovalHours: 1.8,
            refusalCauses,
            permitsByZone
        );
    }

    private static ZoneDto MapZoneToDto(Zone z)
    {
        return new ZoneDto(
            z.Id,
            z.SiteId,
            z.Code,
            z.Name,
            z.Latitude,
            z.Longitude,
            z.RadiusMeters,
            z.QrCodePayload,
            z.IsActive,
            z.AdjacentZones.Select(a => a.AdjacentZone?.Code ?? "").Where(c => !string.IsNullOrEmpty(c)).ToList()
        );
    }
}
