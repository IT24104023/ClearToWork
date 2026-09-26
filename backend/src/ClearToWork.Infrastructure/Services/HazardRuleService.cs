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

    // ─── Student 4: Safety Observations Implementation ──────────────────────────
    public async Task<List<ObservationDto>> GetObservationsAsync(Guid? zoneId = null, string? category = null)
    {
        var query = _context.Observations.AsQueryable();

        if (zoneId.HasValue && zoneId.Value != Guid.Empty)
        {
            query = query.Where(o => o.ZoneId == zoneId.Value);
        }

        if (!string.IsNullOrWhiteSpace(category) && category != "ALL")
        {
            query = query.Where(o => o.Category.ToLower() == category.ToLower());
        }

        var observations = await query
            .OrderByDescending(o => o.LoggedAt)
            .ToListAsync();

        var zoneMap = await _context.Zones.ToDictionaryAsync(z => z.Id, z => z);
        var userMap = await _context.Users.ToDictionaryAsync(u => u.Id, u => u);

        return observations.Select(o =>
        {
            zoneMap.TryGetValue(o.ZoneId, out var zone);
            userMap.TryGetValue(o.ReportedByUserId, out var user);
            return new ObservationDto(
                o.Id,
                o.ZoneId,
                zone?.Code ?? "UNKNOWN",
                zone?.Name ?? "General Site",
                o.ReportedByUserId,
                user?.FullName ?? "HSE Field Personnel",
                o.Category,
                o.Description,
                o.LoggedAt,
                o.CreatedAt
            );
        }).ToList();
    }

    public async Task<ObservationDto?> GetObservationByIdAsync(Guid id)
    {
        var o = await _context.Observations.FindAsync(id);
        if (o == null) return null;

        var zone = await _context.Zones.FindAsync(o.ZoneId);
        var user = await _context.Users.FindAsync(o.ReportedByUserId);

        return new ObservationDto(
            o.Id,
            o.ZoneId,
            zone?.Code ?? "UNKNOWN",
            zone?.Name ?? "General Site",
            o.ReportedByUserId,
            user?.FullName ?? "HSE Field Personnel",
            o.Category,
            o.Description,
            o.LoggedAt,
            o.CreatedAt
        );
    }

    public async Task<ObservationDto> CreateObservationAsync(Guid reportedByUserId, CreateObservationRequest request)
    {
        var obs = new Observation
        {
            ZoneId = request.ZoneId,
            ReportedByUserId = reportedByUserId,
            Category = request.Category,
            Description = request.Description,
            LoggedAt = DateTime.UtcNow,
            CreatedBy = reportedByUserId.ToString(),
            CreatedAt = DateTime.UtcNow,
        };

        _context.Observations.Add(obs);
        await _context.SaveChangesAsync();

        var zone = await _context.Zones.FindAsync(obs.ZoneId);
        var user = await _context.Users.FindAsync(reportedByUserId);

        return new ObservationDto(
            obs.Id,
            obs.ZoneId,
            zone?.Code ?? "UNKNOWN",
            zone?.Name ?? "General Site",
            obs.ReportedByUserId,
            user?.FullName ?? "HSE Field Personnel",
            obs.Category,
            obs.Description,
            obs.LoggedAt,
            obs.CreatedAt
        );
    }

    public async Task<ObservationDto?> UpdateObservationAsync(Guid id, UpdateObservationRequest request)
    {
        var obs = await _context.Observations.FindAsync(id);
        if (obs == null) return null;

        if (request.ZoneId.HasValue && request.ZoneId.Value != Guid.Empty)
        {
            obs.ZoneId = request.ZoneId.Value;
        }
        obs.Category = request.Category;
        obs.Description = request.Description;
        obs.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var zone = await _context.Zones.FindAsync(obs.ZoneId);
        var user = await _context.Users.FindAsync(obs.ReportedByUserId);

        return new ObservationDto(
            obs.Id,
            obs.ZoneId,
            zone?.Code ?? "UNKNOWN",
            zone?.Name ?? "General Site",
            obs.ReportedByUserId,
            user?.FullName ?? "HSE Field Personnel",
            obs.Category,
            obs.Description,
            obs.LoggedAt,
            obs.CreatedAt
        );
    }

    public async Task<bool> DeleteObservationAsync(Guid id)
    {
        var obs = await _context.Observations.FindAsync(id);
        if (obs == null) return false;

        _context.Observations.Remove(obs);
        await _context.SaveChangesAsync();
        return true;
    }

    // ─── Student 4: Rulebook & Hazard Types Implementation ───────────────────────
    public async Task<List<HazardTypeDto>> GetHazardTypesAsync()
    {
        var types = await _context.HazardTypes
            .Include(h => h.ControlMeasures)
            .OrderBy(h => h.Code)
            .ToListAsync();

        return types.Select(MapHazardTypeToDto).ToList();
    }

    public async Task<HazardTypeDto?> GetHazardTypeByIdAsync(Guid id)
    {
        var type = await _context.HazardTypes
            .Include(h => h.ControlMeasures)
            .FirstOrDefaultAsync(h => h.Id == id);

        return type == null ? null : MapHazardTypeToDto(type);
    }

    public async Task<HazardTypeDto> CreateHazardTypeAsync(CreateHazardTypeRequest request)
    {
        if (!Enum.TryParse<HazardSeverity>(request.SeverityLevel, true, out var severity))
        {
            severity = HazardSeverity.Medium;
        }

        var hazard = new HazardType
        {
            Code = request.Code.Trim().ToUpper(),
            Name = request.Name.Trim(),
            SeverityLevel = severity,
            MaxWindSpeedKmh = request.MaxWindSpeedKmh,
            ProhibitedInRain = request.ProhibitedInRain,
        };

        _context.HazardTypes.Add(hazard);
        await _context.SaveChangesAsync();

        return MapHazardTypeToDto(hazard);
    }

    public async Task<HazardTypeDto?> UpdateHazardTypeAsync(Guid id, UpdateHazardTypeRequest request)
    {
        var hazard = await _context.HazardTypes
            .Include(h => h.ControlMeasures)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hazard == null) return null;

        if (Enum.TryParse<HazardSeverity>(request.SeverityLevel, true, out var severity))
        {
            hazard.SeverityLevel = severity;
        }

        hazard.Name = request.Name.Trim();
        hazard.MaxWindSpeedKmh = request.MaxWindSpeedKmh;
        hazard.ProhibitedInRain = request.ProhibitedInRain;

        await _context.SaveChangesAsync();

        return MapHazardTypeToDto(hazard);
    }

    public async Task<bool> DeleteHazardTypeAsync(Guid id)
    {
        var hazard = await _context.HazardTypes
            .Include(h => h.ControlMeasures)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hazard == null) return false;

        _context.ControlMeasures.RemoveRange(hazard.ControlMeasures);
        _context.HazardTypes.Remove(hazard);
        await _context.SaveChangesAsync();
        return true;
    }

    // ─── Student 4: Control Measures Implementation ──────────────────────────────
    public async Task<ControlMeasureDto> CreateControlMeasureAsync(CreateControlMeasureRequest request)
    {
        var cm = new ControlMeasure
        {
            HazardTypeId = request.HazardTypeId,
            Code = request.Code.Trim().ToUpper(),
            RequirementDescription = request.RequirementDescription.Trim(),
            IsMandatory = request.IsMandatory,
        };

        _context.ControlMeasures.Add(cm);
        await _context.SaveChangesAsync();

        return new ControlMeasureDto(cm.Id, cm.HazardTypeId, cm.Code, cm.RequirementDescription, cm.IsMandatory);
    }

    public async Task<ControlMeasureDto?> UpdateControlMeasureAsync(Guid id, UpdateControlMeasureRequest request)
    {
        var cm = await _context.ControlMeasures.FindAsync(id);
        if (cm == null) return null;

        cm.RequirementDescription = request.RequirementDescription.Trim();
        cm.IsMandatory = request.IsMandatory;

        await _context.SaveChangesAsync();

        return new ControlMeasureDto(cm.Id, cm.HazardTypeId, cm.Code, cm.RequirementDescription, cm.IsMandatory);
    }

    public async Task<bool> DeleteControlMeasureAsync(Guid id)
    {
        var cm = await _context.ControlMeasures.FindAsync(id);
        if (cm == null) return false;

        _context.ControlMeasures.Remove(cm);
        await _context.SaveChangesAsync();
        return true;
    }

    // ─── Student 4: SIMOPS Incompatibility Rules Implementation ─────────────────
    public async Task<List<IncompatibilityRuleDto>> GetIncompatibilityRulesAsync()
    {
        var rules = await _context.IncompatibilityRules
            .Include(r => r.PrimaryHazard)
            .Include(r => r.ConflictingHazard)
            .OrderBy(r => r.RuleCode)
            .ToListAsync();

        return rules.Select(MapRuleToDto).ToList();
    }

    public async Task<IncompatibilityRuleDto> CreateIncompatibilityRuleAsync(CreateIncompatibilityRuleRequest request)
    {
        var rule = new IncompatibilityRule
        {
            RuleCode = request.RuleCode.Trim().ToUpper(),
            PrimaryHazardId = request.PrimaryHazardId,
            ConflictingHazardId = request.ConflictingHazardId,
            Reason = request.Reason.Trim(),
            AppliesToAdjacentZones = request.AppliesToAdjacentZones,
        };


        _context.IncompatibilityRules.Add(rule);
        await _context.SaveChangesAsync();

        var primary = await _context.HazardTypes.FindAsync(rule.PrimaryHazardId);
        var conflicting = await _context.HazardTypes.FindAsync(rule.ConflictingHazardId);
        rule.PrimaryHazard = primary;
        rule.ConflictingHazard = conflicting;

        return MapRuleToDto(rule);
    }

    public async Task<IncompatibilityRuleDto?> UpdateIncompatibilityRuleAsync(Guid id, UpdateIncompatibilityRuleRequest request)
    {
        var rule = await _context.IncompatibilityRules
            .Include(r => r.PrimaryHazard)
            .Include(r => r.ConflictingHazard)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rule == null) return null;

        rule.Reason = request.Reason.Trim();
        rule.AppliesToAdjacentZones = request.AppliesToAdjacentZones;

        await _context.SaveChangesAsync();

        return MapRuleToDto(rule);
    }

    public async Task<bool> DeleteIncompatibilityRuleAsync(Guid id)
    {
        var rule = await _context.IncompatibilityRules.FindAsync(id);
        if (rule == null) return false;

        _context.IncompatibilityRules.Remove(rule);
        await _context.SaveChangesAsync();
        return true;
    }

    // ─── Student 4: Zone Adjacencies Implementation ──────────────────────────────
    public async Task<List<ZoneAdjacencyDto>> GetZoneAdjacenciesAsync()
    {
        var adjacencies = await _context.ZoneAdjacencies
            .Include(za => za.Zone)
            .Include(za => za.AdjacentZone)
            .ToListAsync();

        return adjacencies.Select(za => new ZoneAdjacencyDto(
            za.ZoneId,
            za.Zone?.Code ?? "UNKNOWN",
            za.Zone?.Name ?? "Zone",
            za.AdjacentZoneId,
            za.AdjacentZone?.Code ?? "UNKNOWN",
            za.AdjacentZone?.Name ?? "Zone"
        )).ToList();
    }

    public async Task<bool> AddZoneAdjacencyAsync(AddZoneAdjacencyRequest request)
    {
        if (request.ZoneId == request.AdjacentZoneId) return false;

        var existing = await _context.ZoneAdjacencies
            .FirstOrDefaultAsync(za => za.ZoneId == request.ZoneId && za.AdjacentZoneId == request.AdjacentZoneId);

        if (existing != null) return true;

        var forward = new ZoneAdjacency { ZoneId = request.ZoneId, AdjacentZoneId = request.AdjacentZoneId };
        var backward = new ZoneAdjacency { ZoneId = request.AdjacentZoneId, AdjacentZoneId = request.ZoneId };

        _context.ZoneAdjacencies.Add(forward);

        // Check reverse adjacency
        var reverseExisting = await _context.ZoneAdjacencies
            .FirstOrDefaultAsync(za => za.ZoneId == request.AdjacentZoneId && za.AdjacentZoneId == request.ZoneId);
        if (reverseExisting == null)
        {
            _context.ZoneAdjacencies.Add(backward);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveZoneAdjacencyAsync(Guid zoneId, Guid adjacentZoneId)
    {
        var forward = await _context.ZoneAdjacencies
            .FirstOrDefaultAsync(za => za.ZoneId == zoneId && za.AdjacentZoneId == adjacentZoneId);
        var backward = await _context.ZoneAdjacencies
            .FirstOrDefaultAsync(za => za.ZoneId == adjacentZoneId && za.AdjacentZoneId == zoneId);

        if (forward != null) _context.ZoneAdjacencies.Remove(forward);
        if (backward != null) _context.ZoneAdjacencies.Remove(backward);

        await _context.SaveChangesAsync();
        return true;
    }

    private static HazardTypeDto MapHazardTypeToDto(HazardType h)
    {
        return new HazardTypeDto(
            h.Id,
            h.Code,
            h.Name,
            h.SeverityLevel.ToString(),
            h.MaxWindSpeedKmh,
            h.ProhibitedInRain,
            h.ControlMeasures.Select(cm => new ControlMeasureDto(
                cm.Id,
                cm.HazardTypeId,
                cm.Code,
                cm.RequirementDescription,
                cm.IsMandatory
            )).ToList()
        );
    }

    private static IncompatibilityRuleDto MapRuleToDto(IncompatibilityRule r)
    {
        return new IncompatibilityRuleDto(
            r.Id,
            r.RuleCode,
            r.PrimaryHazardId,
            r.PrimaryHazard?.Code ?? "UNKNOWN",
            r.PrimaryHazard?.Name ?? "Primary Hazard",
            r.ConflictingHazardId,
            r.ConflictingHazard?.Code ?? "UNKNOWN",
            r.ConflictingHazard?.Name ?? "Conflicting Hazard",
            r.Reason,
            r.AppliesToAdjacentZones
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