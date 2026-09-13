using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkforceController : ControllerBase
{
    private readonly IWorkforceService _workforceService;

    public WorkforceController(IWorkforceService workforceService)
    {
        _workforceService = workforceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkerDto>>> GetWorkers([FromQuery] string? trade, [FromQuery] bool? activeOnly = true)
    {
        var workers = await _workforceService.GetAllWorkersAsync(trade, activeOnly);
        return Ok(workers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkerDto>> GetWorker(Guid id)
    {
        var worker = await _workforceService.GetWorkerByIdAsync(id);
        if (worker == null) return NotFound(new { message = "Worker not found." });
        return Ok(worker);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,AreaSupervisor")]
    public async Task<ActionResult<WorkerDto>> CreateWorker([FromBody] CreateWorkerRequest request)
    {
        var created = await _workforceService.CreateWorkerAsync(
            request.FirstName, request.LastName, request.BadgeNumber, request.Trade, request.ContractorId);
        return CreatedAtAction(nameof(GetWorker), new { id = created.Id }, created);
    }

    [HttpPost("eligibility-check")]
    public async Task<ActionResult<EligibilityCheckResponse>> CheckEligibility([FromBody] EligibilityCheckRequest request)
    {
        var result = await _workforceService.CheckEligibilityAsync(request);
        return Ok(result);
    }

    [HttpGet("expiry-forecast")]
    [Authorize(Roles = "SafetyOfficer,Administrator,AreaSupervisor")]
    public async Task<ActionResult<List<ExpiryForecastItem>>> GetExpiryForecast()
    {
        var forecast = await _workforceService.Get30DayExpiryForecastAsync();
        return Ok(forecast);
    }
}

public record CreateWorkerRequest(string FirstName, string LastName, string BadgeNumber, string Trade, Guid ContractorId);
