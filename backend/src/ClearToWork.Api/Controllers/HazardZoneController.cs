using System.Security.Claims;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HazardZoneController : ControllerBase
{
    private readonly IHazardRuleService _hazardRuleService;
    private readonly IWeatherService _weatherService;

    public HazardZoneController(IHazardRuleService hazardRuleService, IWeatherService weatherService)
    {
        _hazardRuleService = hazardRuleService;
        _weatherService = weatherService;
    }

    [HttpGet("zones")]
    public async Task<ActionResult<List<ZoneDto>>> GetZones()
    {
        var zones = await _hazardRuleService.GetAllZonesAsync();
        return Ok(zones);
    }

    [HttpGet("zones/{code}")]
    public async Task<ActionResult<ZoneDto>> GetZoneByCode(string code)
    {
        var zone = await _hazardRuleService.GetZoneByCodeAsync(code);
        if (zone == null) return NotFound(new { message = "Zone not found." });
        return Ok(zone);
    }

    [HttpPost("zones/conflict-check")]
    public async Task<ActionResult<ZoneConflictCheckResponse>> CheckZoneConflicts([FromBody] ZoneConflictCheckRequest request)
    {
        var result = await _hazardRuleService.CheckZoneConflictsAsync(request);
        return Ok(result);
    }

    [HttpGet("weather/forecast")]
    public async Task<ActionResult<WeatherForecastDto>> GetWeatherForecast(
        [FromQuery] decimal latitude, [FromQuery] decimal longitude, [FromQuery] DateTime? targetTime)
    {
        var time = targetTime ?? DateTime.UtcNow;
        var forecast = await _weatherService.GetForecastAsync(latitude, longitude, time);
        return Ok(forecast);
    }

    [HttpGet("analytics/safety-summary")]
    [Authorize]
    public async Task<ActionResult<AnalyticsSafetySummaryDto>> GetSafetySummary()
    {
        var summary = await _hazardRuleService.GetSafetyAnalyticsAsync();
        return Ok(summary);
    }

    // ─── Student 4: Safety Observations Endpoints ────────────────────────────────
    [HttpGet("observations")]
    public async Task<ActionResult<List<ObservationDto>>> GetObservations([FromQuery] Guid? zoneId, [FromQuery] string? category)
    {
        var observations = await _hazardRuleService.GetObservationsAsync(zoneId, category);
        return Ok(observations);
    }

    [HttpGet("observations/{id:guid}")]
    public async Task<ActionResult<ObservationDto>> GetObservation(Guid id)
    {
        var observation = await _hazardRuleService.GetObservationByIdAsync(id);
        if (observation == null) return NotFound(new { message = "Observation not found." });
        return Ok(observation);
    }

    [HttpPost("observations")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor,ContractorSupervisor")]
    public async Task<ActionResult<ObservationDto>> CreateObservation([FromBody] CreateObservationRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var created = await _hazardRuleService.CreateObservationAsync(userId, request);
        return CreatedAtAction(nameof(GetObservation), new { id = created.Id }, created);
    }

    [HttpPut("observations/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor,ContractorSupervisor")]
    public async Task<ActionResult<ObservationDto>> UpdateObservation(Guid id, [FromBody] UpdateObservationRequest request)
    {
        var updated = await _hazardRuleService.UpdateObservationAsync(id, request);
        if (updated == null) return NotFound(new { message = "Observation not found." });
        return Ok(updated);
    }

    [HttpDelete("observations/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor")]
    public async Task<IActionResult> DeleteObservation(Guid id)
    {
        var success = await _hazardRuleService.DeleteObservationAsync(id);
        if (!success) return NotFound(new { message = "Observation not found." });
        return Ok(new { message = "Observation deleted successfully.", id });
    }

    // ─── Student 4: Rulebook Hazard Types Endpoints ──────────────────────────────
    [HttpGet("hazard-types")]
    public async Task<ActionResult<List<HazardTypeDto>>> GetHazardTypes()
    {
        var types = await _hazardRuleService.GetHazardTypesAsync();
        return Ok(types);
    }

    [HttpGet("hazard-types/{id:guid}")]
    public async Task<ActionResult<HazardTypeDto>> GetHazardType(Guid id)
    {
        var type = await _hazardRuleService.GetHazardTypeByIdAsync(id);
        if (type == null) return NotFound(new { message = "Hazard type not found." });
        return Ok(type);
    }

    [HttpPost("hazard-types")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor")]
    public async Task<ActionResult<HazardTypeDto>> CreateHazardType([FromBody] CreateHazardTypeRequest request)
    {
        var created = await _hazardRuleService.CreateHazardTypeAsync(request);
        return CreatedAtAction(nameof(GetHazardType), new { id = created.Id }, created);
    }

    [HttpPut("hazard-types/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor")]
    public async Task<ActionResult<HazardTypeDto>> UpdateHazardType(Guid id, [FromBody] UpdateHazardTypeRequest request)
    {
        var updated = await _hazardRuleService.UpdateHazardTypeAsync(id, request);
        if (updated == null) return NotFound(new { message = "Hazard type not found." });
        return Ok(updated);
    }

    [HttpDelete("hazard-types/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer")]
    public async Task<IActionResult> DeleteHazardType(Guid id)
    {
        var success = await _hazardRuleService.DeleteHazardTypeAsync(id);
        if (!success) return NotFound(new { message = "Hazard type not found." });
        return Ok(new { message = "Hazard type deleted successfully.", id });
    }

    // ─── Student 4: Control Measures Endpoints ───────────────────────────────────
    [HttpPost("control-measures")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor")]
    public async Task<ActionResult<ControlMeasureDto>> CreateControlMeasure([FromBody] CreateControlMeasureRequest request)
    {
        var created = await _hazardRuleService.CreateControlMeasureAsync(request);
        return Ok(created);
    }

    [HttpPut("control-measures/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor")]
    public async Task<ActionResult<ControlMeasureDto>> UpdateControlMeasure(Guid id, [FromBody] UpdateControlMeasureRequest request)
    {
        var updated = await _hazardRuleService.UpdateControlMeasureAsync(id, request);
        if (updated == null) return NotFound(new { message = "Control measure not found." });
        return Ok(updated);
    }

    [HttpDelete("control-measures/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer")]
    public async Task<IActionResult> DeleteControlMeasure(Guid id)
    {
        var success = await _hazardRuleService.DeleteControlMeasureAsync(id);
        if (!success) return NotFound(new { message = "Control measure not found." });
        return Ok(new { message = "Control measure deleted successfully.", id });
    }

    // ─── Student 4: SIMOPS Incompatibility Rules Endpoints ───────────────────────
    [HttpGet("incompatibility-rules")]
    public async Task<ActionResult<List<IncompatibilityRuleDto>>> GetIncompatibilityRules()
    {
        var rules = await _hazardRuleService.GetIncompatibilityRulesAsync();
        return Ok(rules);
    }

    [HttpPost("incompatibility-rules")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor")]
    public async Task<ActionResult<IncompatibilityRuleDto>> CreateIncompatibilityRule([FromBody] CreateIncompatibilityRuleRequest request)
    {
        var created = await _hazardRuleService.CreateIncompatibilityRuleAsync(request);
        return Ok(created);
    }

    [HttpPut("incompatibility-rules/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer,AreaSupervisor")]
    public async Task<ActionResult<IncompatibilityRuleDto>> UpdateIncompatibilityRule(Guid id, [FromBody] UpdateIncompatibilityRuleRequest request)
    {
        var updated = await _hazardRuleService.UpdateIncompatibilityRuleAsync(id, request);
        if (updated == null) return NotFound(new { message = "Incompatibility rule not found." });
        return Ok(updated);
    }

    [HttpDelete("incompatibility-rules/{id:guid}")]
    [Authorize(Roles = "Administrator,SafetyOfficer")]
    public async Task<IActionResult> DeleteIncompatibilityRule(Guid id)
    {
        var success = await _hazardRuleService.DeleteIncompatibilityRuleAsync(id);
        if (!success) return NotFound(new { message = "Incompatibility rule not found." });
        return Ok(new { message = "Incompatibility rule deleted successfully.", id });
    }

    // ─── Student 4: Zone Adjacencies Endpoints ───────────────────────────────────
    [HttpGet("zone-adjacencies")]
    public async Task<ActionResult<List<ZoneAdjacencyDto>>> GetZoneAdjacencies()
    {
        var adjacencies = await _hazardRuleService.GetZoneAdjacenciesAsync();
        return Ok(adjacencies);
    }

    [HttpPost("zone-adjacencies")]
    [Authorize(Roles = "Administrator,AreaSupervisor,SafetyOfficer")]
    public async Task<IActionResult> AddZoneAdjacency([FromBody] AddZoneAdjacencyRequest request)
    {
        var success = await _hazardRuleService.AddZoneAdjacencyAsync(request);
        if (!success) return BadRequest(new { message = "Could not link adjacent zones." });
        return Ok(new { message = "Zone adjacency link established successfully." });
    }

    [HttpDelete("zone-adjacencies/{zoneId:guid}/{adjacentZoneId:guid}")]
    [Authorize(Roles = "Administrator,AreaSupervisor,SafetyOfficer")]
    public async Task<IActionResult> RemoveZoneAdjacency(Guid zoneId, Guid adjacentZoneId)
    {
        var success = await _hazardRuleService.RemoveZoneAdjacencyAsync(zoneId, adjacentZoneId);
        if (!success) return NotFound(new { message = "Zone adjacency link not found." });
        return Ok(new { message = "Zone adjacency link removed successfully." });
    }
}

