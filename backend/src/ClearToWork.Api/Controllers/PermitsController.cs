using System.Security.Claims;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermitsController : ControllerBase
{
    private readonly IPermitLifecycleService _permitService;
    private readonly AppDbContext _context;

    public PermitsController(IPermitLifecycleService permitService, AppDbContext context)
    {
        _permitService = permitService;
        _context = context;
    }

    /// <summary>Returns all permit types (Id, Code, Name) for the New Permit form selector.</summary>
    [HttpGet("types")]
    public async Task<IActionResult> GetPermitTypes()
    {
        var types = await _context.PermitTypes
            .Select(pt => new { pt.Id, pt.Code, pt.Name, pt.MaxDurationHours, pt.RequiresFireWatch, pt.RequiresGasTesting })
            .ToListAsync();
        return Ok(types);
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ContractorSupervisor,AreaSupervisor,Administrator")]
    public async Task<ActionResult<PermitDetailsDto>> UpdatePermitDraft(Guid id, [FromBody] UpdatePermitRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var supervisorId))
        {
            return Unauthorized();
        }

        try
        {
            var updated = await _permitService.UpdatePermitDraftAsync(id, supervisorId, request);
            if (updated == null) return NotFound(new { message = "Permit not found." });
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "ContractorSupervisor,AreaSupervisor,Administrator")]
    public async Task<IActionResult> DeletePermitDraft(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var supervisorId))
        {
            return Unauthorized();
        }

        try
        {
            var success = await _permitService.DeletePermitDraftAsync(id, supervisorId);
            if (!success) return NotFound(new { message = "Permit not found." });
            return Ok(new { message = "Permit draft successfully deleted.", id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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