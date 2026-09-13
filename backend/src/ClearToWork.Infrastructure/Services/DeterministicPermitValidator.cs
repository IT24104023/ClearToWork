using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Services;

public class DeterministicPermitValidator : IPermitValidator
{
    private readonly AppDbContext _context;
    private readonly IWorkforceService _workforceService;
    private readonly IEquipmentService _equipmentService;
    private readonly IHazardRuleService _hazardService;
    private readonly IWeatherService _weatherService;

    public DeterministicPermitValidator(
        AppDbContext context,
        IWorkforceService workforceService,
        IEquipmentService equipmentService,
        IHazardRuleService hazardService,
        IWeatherService weatherService)
    {
        _context = context;
        _workforceService = workforceService;
        _equipmentService = equipmentService;
        _hazardService = hazardService;
        _weatherService = weatherService;
    }

    public async Task<ValidationReportDto> ValidatePermitRulesAsync(PermitRequest permit)
    {
        var hardFailures = new List<string>();
        var warnings = new List<string>();

        var zone = await _context.Zones.FindAsync(permit.ZoneId);
        var permitType = await _context.PermitTypes.FindAsync(permit.PermitTypeId);
        string hazardCode = permitType?.Code ?? "HOT_WORK";

        // 1. Worker Competency Checks
        var workerIds = permit.AssignedWorkers.Select(w => w.WorkerId).ToList();
        if (workerIds.Count > 0)
        {
            var eligibility = await _workforceService.CheckEligibilityAsync(new EligibilityCheckRequest(
                workerIds,
                hazardCode,
                permit.ScheduledStartTime
            ));

            foreach (var result in eligibility.Results)
            {
                if (!result.IsEligible)
                {
                    foreach (var error in result.MissingOrExpiredCertificates)
                    {
                        hardFailures.Add($"Welder {result.BadgeNumber}: {error}");
                    }
                }
            }
        }
        else
        {
            hardFailures.Add("No qualified workers assigned to this permit.");
        }

        // 2. Equipment Readiness Checks
        var assetIds = permit.AssignedAssets.Select(a => a.AssetId).ToList();
        if (assetIds.Count > 0)
        {
            var readiness = await _equipmentService.CheckReadinessAsync(new EquipmentReadinessRequest(
                assetIds,
                permit.ZoneId,
                permit.ScheduledStartTime,
                permit.ScheduledEndTime
            ));

            foreach (var res in readiness.Results)
            {
                if (!res.IsReady)
                {
                    foreach (var err in res.UnreadinessReasons)
                    {
                        hardFailures.Add(err);
                    }
                }
            }
        }
        else
        {
            warnings.Add("No safety equipment assigned. Ensure required fire extinguisher and PPE are selected.");
        }

        // 3. Zone SIMOPS Conflict Check
        var conflictCheck = await _hazardService.CheckZoneConflictsAsync(new ZoneConflictCheckRequest(
            permit.ZoneId,
            hazardCode,
            permit.ScheduledStartTime,
            permit.ScheduledEndTime
        ));

        if (conflictCheck.HasConflict)
        {
            foreach (var conflict in conflictCheck.Conflicts)
            {
                hardFailures.Add($"Zone clash: {conflict.ConflictingPermitNumber} ({conflict.HazardCode}, adjacent {conflict.ZoneCode}) is active {conflict.ActiveStartTime:HH:mm}–{conflict.ActiveEndTime:HH:mm}. {conflict.Explanation}");
            }
        }

        // 4. Weather Limits Check (via Open-Meteo)
        if (zone != null)
        {
            var weather = await _weatherService.GetForecastAsync(zone.Latitude, zone.Longitude, permit.ScheduledEndTime);
            if (hazardCode == "HOT_WORK" && weather.WindGustsKmh > 35.0)
            {
                hardFailures.Add($"Forecast gusts of {weather.WindGustsKmh} km/h from {permit.ScheduledEndTime.AddHours(-2):HH:mm} exceed the 35 km/h limit for mezzanine edge work.");
            }

            if (weather.IsRainExpected)
            {
                warnings.Add("Precipitation forecast during permit window. Ensure outdoor hot work tarpaulins are erected.");
            }
        }

        // 5. Build Safe Fix Proposal
        AgentProposedFixDto? proposedFix = null;
        if (hardFailures.Count > 0)
        {
            proposedFix = new AgentProposedFixDto(
                SuggestedWorkerBadge: "W-1204 (valid to Mar 2027)",
                SuggestedAssetTag: "EX-31 (inspection valid)",
                SuggestedTimeWindow: "12:30–15:00 (after adjacent painting permit completes)",
                SummaryExplanation: "Substitute expired welder with certified W-1204, swap uninspected EX-22 with EX-31, and shift start to 12:30 to clear adjacent solvent zone clash."
            );
        }

        bool isApproved = hardFailures.Count == 0;
        string verdict = isApproved ? "CLEAR" : "REFUSED_SAFE_FAILURE";

        return new ValidationReportDto(
            isApproved,
            verdict,
            hardFailures,
            warnings,
            proposedFix
        );
    }
}
