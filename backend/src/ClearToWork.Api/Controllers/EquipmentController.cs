using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<AssetDto>>> GetEquipment([FromQuery] string? category, [FromQuery] string? status)
    {
        var assets = await _equipmentService.GetAllAssetsAsync(category, status);
        return Ok(assets);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AssetDto>> GetEquipmentById(Guid id)
    {
        var asset = await _equipmentService.GetAssetByIdAsync(id);
        if (asset == null) return NotFound(new { message = "Asset not found." });
        return Ok(asset);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<AssetDto>> CreateAsset([FromBody] CreateAssetRequest request)
    {
        var created = await _equipmentService.CreateAssetAsync(request);
        return CreatedAtAction(nameof(GetEquipmentById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AssetDto>> UpdateAsset(Guid id, [FromBody] UpdateAssetRequest request)
    {
        var updated = await _equipmentService.UpdateAssetAsync(id, request);
        if (updated == null) return NotFound(new { message = "Asset not found." });
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> DeleteAsset(Guid id)
    {
        var success = await _equipmentService.DeleteAssetAsync(id);
        if (!success) return NotFound(new { message = "Asset not found." });
        return Ok(new { message = "Asset deleted successfully.", id });
    }

    [HttpPost("{id:guid}/inspections")]
    [AllowAnonymous]
    public async Task<IActionResult> AddInspection(Guid id, [FromBody] CreateInspectionRequest request)
    {
        var success = await _equipmentService.AddInspectionRecordAsync(id, request);
        if (!success) return NotFound(new { message = "Asset not found." });
        return Ok(new { message = "Inspection record logged successfully." });
    }

    [HttpPost("{id:guid}/calibrations")]
    [AllowAnonymous]
    public async Task<IActionResult> AddCalibration(Guid id, [FromBody] CreateCalibrationRequest request)
    {
        var success = await _equipmentService.AddCalibrationRecordAsync(id, request);
        if (!success) return NotFound(new { message = "Asset not found." });
        return Ok(new { message = "Calibration record logged successfully." });
    }

    [HttpPost("readiness-check")]
    [AllowAnonymous]
    public async Task<ActionResult<EquipmentReadinessResponse>> CheckReadiness([FromBody] EquipmentReadinessRequest request)
    {
        var response = await _equipmentService.CheckReadinessAsync(request);
        return Ok(response);
    }

    [HttpPost("check-tags")]
    [HttpPost("/api/internal/check-equipment-tags")]
    [AllowAnonymous]
    public async Task<ActionResult<EquipmentReadinessResponse>> CheckReadinessByTags([FromBody] CheckEquipmentTagsRequest request)
    {
        var response = await _equipmentService.CheckReadinessByTagsAsync(request.AssetTags);
        return Ok(response);
    }

    [HttpGet("by-tag/{tag}")]
    [AllowAnonymous]
    public async Task<ActionResult<EquipmentReadinessResponse>> CheckReadinessBySingleTag(string tag)
    {
        var response = await _equipmentService.CheckReadinessByTagsAsync(new List<string> { tag });
        return Ok(response);
    }

    [HttpGet("isolation-points/{zoneId}")]
    [HttpGet("isolation-points")]
    [HttpGet("/api/internal/isolation-points/{zoneId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<IsolationPointDto>>> GetIsolationPoints(string? zoneId = null)
    {
        Guid parsedZoneId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(zoneId) && Guid.TryParse(zoneId, out var g))
        {
            parsedZoneId = g;
        }

        var points = await _equipmentService.GetIsolationPointsForZoneAsync(parsedZoneId);
        return Ok(points);
    }

    [HttpPost("isolation-points")]
    [AllowAnonymous]
    public async Task<ActionResult<IsolationPointDto>> CreateIsolationPoint([FromBody] CreateIsolationPointRequest request)
    {
        var point = await _equipmentService.CreateIsolationPointAsync(request);
        return Ok(point);
    }

    [HttpPut("isolation-points/{id:guid}/state")]
    [AllowAnonymous]
    public async Task<ActionResult<IsolationPointDto>> UpdateIsolationPointState(Guid id, [FromBody] UpdateIsolationPointStateRequest request)
    {
        var updated = await _equipmentService.UpdateIsolationPointStateAsync(id, request);
        if (updated == null) return NotFound(new { message = "Isolation point not found." });
        return Ok(updated);
    }
}