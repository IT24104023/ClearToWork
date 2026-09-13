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
    [Authorize(Roles = "SafetyOfficer,Administrator,AreaSupervisor")]
    public async Task<ActionResult<AnalyticsSafetySummaryDto>> GetSafetySummary()
    {
        var summary = await _hazardRuleService.GetSafetyAnalyticsAsync();
        return Ok(summary);
    }
}
