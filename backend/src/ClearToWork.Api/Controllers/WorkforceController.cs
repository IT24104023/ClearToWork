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
    [Authorize(Roles = "Administrator,AreaSupervisor,ContractorSupervisor")]
    public async Task<ActionResult<WorkerDto>> CreateWorker([FromBody] CreateWorkerRequest request)
    {
        var created = await _workforceService.CreateWorkerAsync(
            request.FirstName, request.LastName, request.BadgeNumber, request.Trade, request.ContractorId);
        return CreatedAtAction(nameof(GetWorker), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator,AreaSupervisor,ContractorSupervisor")]
    public async Task<ActionResult<WorkerDto>> UpdateWorker(Guid id, [FromBody] UpdateWorkerRequest request)
    {
        var updated = await _workforceService.UpdateWorkerAsync(id, request);
        if (updated == null) return NotFound(new { message = "Worker not found." });
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator,AreaSupervisor")]
    public async Task<IActionResult> DeleteWorker(Guid id)
    {
        var success = await _workforceService.DeleteWorkerAsync(id);
        if (!success) return NotFound(new { message = "Worker not found." });
        return Ok(new { message = "Worker deleted successfully.", id });
    }

    [HttpPost("{id:guid}/certificates")]
    [Authorize(Roles = "Administrator,AreaSupervisor,ContractorSupervisor,SafetyOfficer")]
    public async Task<ActionResult<WorkerCertificateDto>> AddCertificate(Guid id, [FromBody] CreateCertificateRequest request)
    {
        var cert = await _workforceService.AddWorkerCertificateAsync(id, request);
        if (cert == null) return NotFound(new { message = "Worker or Certificate Type not found." });
        return Ok(cert);
    }

    [HttpDelete("certificates/{certificateId:guid}")]
    [Authorize(Roles = "Administrator,AreaSupervisor,SafetyOfficer")]
    public async Task<IActionResult> DeleteCertificate(Guid certificateId)
    {
        var success = await _workforceService.DeleteWorkerCertificateAsync(certificateId);
        if (!success) return NotFound(new { message = "Certificate not found." });
        return Ok(new { message = "Certificate removed successfully.", certificateId });
    }

    [HttpGet("contractors")]
    public async Task<ActionResult<List<ContractorDto>>> GetContractors()
    {
        var contractors = await _workforceService.GetContractorsAsync();
        return Ok(contractors);
    }

    [HttpGet("certificate-types")]
    public async Task<ActionResult<List<CertificateTypeDto>>> GetCertificateTypes()
    {
        var types = await _workforceService.GetCertificateTypesAsync();
        return Ok(types);
    }

    [HttpPost("eligibility-check")]
    public async Task<ActionResult<EligibilityCheckResponse>> CheckEligibility([FromBody] EligibilityCheckRequest request)
    {
        var result = await _workforceService.CheckEligibilityAsync(request);
        return Ok(result);
    }

    [HttpGet("expiry-forecast")]
    [Authorize]
    public async Task<ActionResult<List<ExpiryForecastItem>>> GetExpiryForecast()
    {
        var forecast = await _workforceService.Get30DayExpiryForecastAsync();
        return Ok(forecast);
    }
}