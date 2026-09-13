using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/internal")]
public class InternalAgentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWorkforceService _workforceService;
    private readonly IEquipmentService _equipmentService;
    private readonly IHazardRuleService _hazardRuleService;
    private readonly IWeatherService _weatherService;
    private readonly IPermitValidator _validator;
    private readonly IConfiguration _config;

    public InternalAgentController(
        AppDbContext context,
        IWorkforceService workforceService,
        IEquipmentService equipmentService,
        IHazardRuleService hazardRuleService,
        IWeatherService weatherService,
        IPermitValidator validator,
        IConfiguration config)
    {
        _context = context;
        _workforceService = workforceService;
        _equipmentService = equipmentService;
        _hazardRuleService = hazardRuleService;
        _weatherService = weatherService;
        _validator = validator;
        _config = config;
    }

    private bool ValidateSharedSecret()
    {
        var configuredSecret = _config["AgentService:SharedSecret"] ?? "ClearToWork_Internal_Agent_Key_2026";
        if (Request.Headers.TryGetValue("X-Agent-Secret", out var headerSecret))
        {
            return headerSecret == configuredSecret;
        }
        return false;
    }

    [HttpGet("permit-template/{code}")]
    public async Task<IActionResult> GetPermitTypeTemplate(string code)
    {
        if (!ValidateSharedSecret()) return Unauthorized(new { message = "Invalid internal agent secret." });

        var permitType = await _context.PermitTypes
            .FirstOrDefaultAsync(p => p.Code.ToLower() == code.ToLower());

        if (permitType == null) return NotFound(new { message = "Permit type not found." });

        return Ok(new
        {
            permitType.Id,
            permitType.Code,
            permitType.Name,
            permitType.Description,
            permitType.MaxDurationHours,
            permitType.RequiresFireWatch
        });
    }

    [HttpGet("worker-certificates/{workerId:guid}")]
    public async Task<IActionResult> GetWorkerCertificates(Guid workerId)
    {
        if (!ValidateSharedSecret()) return Unauthorized(new { message = "Invalid internal agent secret." });

        var worker = await _workforceService.GetWorkerByIdAsync(workerId);
        if (worker == null) return NotFound(new { message = "Worker not found." });

        return Ok(worker.Certificates);
    }

    [HttpPost("check-equipment")]
    public async Task<IActionResult> CheckEquipment([FromBody] EquipmentReadinessRequest request)
    {
        if (!ValidateSharedSecret()) return Unauthorized(new { message = "Invalid internal agent secret." });

        var result = await _equipmentService.CheckReadinessAsync(request);
        return Ok(result);
    }

    [HttpPost("check-zone-conflicts")]
    public async Task<IActionResult> CheckZoneConflicts([FromBody] ZoneConflictCheckRequest request)
    {
        if (!ValidateSharedSecret()) return Unauthorized(new { message = "Invalid internal agent secret." });

        var result = await _hazardRuleService.CheckZoneConflictsAsync(request);
        return Ok(result);
    }

    [HttpGet("weather-forecast")]
    public async Task<IActionResult> GetWeatherForecast([FromQuery] decimal latitude, [FromQuery] decimal longitude, [FromQuery] DateTime? targetTime)
    {
        if (!ValidateSharedSecret()) return Unauthorized(new { message = "Invalid internal agent secret." });

        var result = await _weatherService.GetForecastAsync(latitude, longitude, targetTime ?? DateTime.UtcNow);
        return Ok(result);
    }

    [HttpPost("run-deterministic-validator/{permitId:guid}")]
    public async Task<IActionResult> RunDeterministicValidator(Guid permitId)
    {
        if (!ValidateSharedSecret()) return Unauthorized(new { message = "Invalid internal agent secret." });

        var permit = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers)
            .Include(p => p.AssignedAssets)
            .FirstOrDefaultAsync(p => p.Id == permitId);

        if (permit == null) return NotFound(new { message = "Permit not found." });

        var report = await _validator.ValidatePermitRulesAsync(permit);
        return Ok(report);
    }
}
