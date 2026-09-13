using System.Security.Claims;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermitsController : ControllerBase
{
    private readonly IPermitLifecycleService _permitService;

    public PermitsController(IPermitLifecycleService permitService)
    {
        _permitService = permitService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PermitDetailsDto>>> GetPermits(
        [FromQuery] string? status, [FromQuery] Guid? contractorId, [FromQuery] Guid? zoneId)
    {
        var permits = await _permitService.GetPermitsAsync(status, contractorId, zoneId);
        return Ok(permits);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermitDetailsDto>> GetPermitById(Guid id)
    {
        var permit = await _permitService.GetPermitByIdAsync(id);
        if (permit == null) return NotFound(new { message = "Permit not found." });
        return Ok(permit);
    }

    [HttpPost]
    [Authorize(Roles = "ContractorSupervisor,AreaSupervisor,Administrator")]
    public async Task<ActionResult<PermitDetailsDto>> CreatePermitDraft([FromBody] CreatePermitRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var supervisorId))
        {
            return Unauthorized();
        }

        var created = await _permitService.CreatePermitDraftAsync(supervisorId, request);
        return CreatedAtAction(nameof(GetPermitById), new { id = created.Id }, created);
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = "ContractorSupervisor,AreaSupervisor,Administrator")]
    public async Task<ActionResult<ValidationReportDto>> SubmitPermit(Guid id)
    {
        try
        {
            var report = await _permitService.SubmitPermitForAiReviewAsync(id);
            return Ok(report);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/decision")]
    [Authorize(Roles = "SafetyOfficer,Administrator")]
    public async Task<ActionResult<PermitDetailsDto>> RecordDecision(Guid id, [FromBody] PermitDecisionRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var safetyOfficerId))
        {
            return Unauthorized();
        }

        try
        {
            var updated = await _permitService.RecordDecisionAsync(id, safetyOfficerId, request);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "ContractorSupervisor,AreaSupervisor")]
    public async Task<IActionResult> ActivatePermit(Guid id, [FromBody] PermitActivationRequest request)
    {
        bool success = await _permitService.ActivatePermitOnSiteAsync(id, request);
        if (!success)
        {
            return BadRequest(new { message = "Permit activation failed. Scanned QR must match the physical zone board, and worker location must be within zone boundary." });
        }

        return Ok(new { message = "Permit successfully activated on-site.", activated = true });
    }

    [HttpPost("{id:guid}/close-out")]
    [Authorize(Roles = "ContractorSupervisor,AreaSupervisor,SafetyOfficer")]
    public async Task<IActionResult> CloseOutPermit(Guid id, [FromBody] PermitCloseOutRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        bool success = await _permitService.CloseOutPermitAsync(id, userId, request);
        if (!success)
        {
            return BadRequest(new { message = "Permit closeout failed. Permit must be active, site cleaned, tools removed, and isolations restored." });
        }

        return Ok(new { message = "Permit successfully closed out.", closed = true });
    }
}
