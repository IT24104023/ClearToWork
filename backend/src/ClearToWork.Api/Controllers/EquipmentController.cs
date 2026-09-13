using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<List<AssetDto>>> GetEquipment([FromQuery] string? category, [FromQuery] string? status)
    {
        var assets = await _equipmentService.GetAllAssetsAsync(category, status);
        return Ok(assets);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssetDto>> GetEquipmentById(Guid id)
    {
        var asset = await _equipmentService.GetAssetByIdAsync(id);
        if (asset == null) return NotFound(new { message = "Asset not found." });
        return Ok(asset);
    }

    [HttpPost("readiness-check")]
    public async Task<ActionResult<EquipmentReadinessResponse>> CheckReadiness([FromBody] EquipmentReadinessRequest request)
    {
        var response = await _equipmentService.CheckReadinessAsync(request);
        return Ok(response);
    }

    [HttpGet("isolation-points/{zoneId:guid}")]
    public async Task<ActionResult<List<IsolationPointDto>>> GetIsolationPoints(Guid zoneId)
    {
        var points = await _equipmentService.GetIsolationPointsForZoneAsync(zoneId);
        return Ok(points);
    }
}
