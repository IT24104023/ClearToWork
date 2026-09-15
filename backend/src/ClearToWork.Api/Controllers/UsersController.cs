using System.Security.Claims;
using ClearToWork.Application.DTOs;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    private static List<string> GetRolePermissions(UserRole role)
    {
        return role switch
        {
            UserRole.Administrator => new List<string>
            {
                "Permit:View", "Permit:Create", "Permit:Submit", "Permit:Approve", "Permit:Reject",
                "Permit:Activate", "Permit:CloseOut", "Workforce:View", "Workforce:Manage",
                "Equipment:View", "Equipment:Manage", "Hazard:View", "Hazard:Manage",
                "Agent:Inspect", "Agent:Simulate", "Agent:Configure",
                "Database:View", "Database:Seed", "Database:Reset", "Database:Export",
                "Users:Manage", "Security:Audit"
            },
            UserRole.SafetyOfficer => new List<string>
            {
                "Permit:View", "Permit:Approve", "Permit:Reject", "Permit:Audit",
                "Workforce:View", "Workforce:Certificates", "Equipment:View", "Equipment:Inspect",
                "Hazard:View", "Hazard:Monitor", "Agent:Inspect", "Agent:Simulate", "Security:Audit"
            },
            UserRole.AreaSupervisor => new List<string>
            {
                "Permit:View", "Permit:Create", "Permit:Submit", "Permit:Activate", "Permit:CloseOut",
                "Workforce:View", "Equipment:View", "Hazard:View", "Agent:Inspect"
            },
            UserRole.ContractorSupervisor => new List<string>
            {
                "Permit:View", "Permit:Create", "Permit:Submit", "Permit:Activate", "Permit:CloseOut",
                "Workforce:View", "Equipment:View", "Hazard:View"
            },
            _ => new List<string> { "Permit:View" }
        };
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(new UserProfileDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.ContractorId,
            user.AvatarUrl,
            user.PhoneNumber,
            user.Department,
            user.Bio,
            GetRolePermissions(user.Role)
        ));
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        user.FullName = request.FullName.Trim();
        user.AvatarUrl = request.AvatarUrl?.Trim();
        user.PhoneNumber = request.PhoneNumber?.Trim();
        user.Department = request.Department?.Trim();
        user.Bio = request.Bio?.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new UserProfileDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.ContractorId,
            user.AvatarUrl,
            user.PhoneNumber,
            user.Department,
            user.Bio,
            GetRolePermissions(user.Role)
        ));
    }

    [HttpGet("permissions")]
    public async Task<ActionResult<List<string>>> GetMyPermissions()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(GetRolePermissions(user.Role));
    }

    [HttpGet]
    [Authorize(Roles = "Administrator,SafetyOfficer")]
    public async Task<ActionResult<List<UserProfileDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .OrderBy(u => u.FullName)
            .Select(u => new UserProfileDto(
                u.Id,
                u.FullName,
                u.Email,
                u.Role.ToString(),
                u.ContractorId,
                u.AvatarUrl,
                u.PhoneNumber,
                u.Department,
                u.Bio,
                GetRolePermissions(u.Role)
            ))
            .ToListAsync();

        return Ok(users);
    }
}
