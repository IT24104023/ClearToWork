using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("profile")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        User? user = null;
        if (Guid.TryParse(userIdStr, out var userId))
        {
            user = await _context.Users.FindAsync(userId);
        }

        if (user == null)
        {
            var emailClaim = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(emailClaim))
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim);
            }
        }

        if (user == null)
        {
            user = await _context.Users.FirstOrDefaultAsync();
        }

        var resolvedFullName = user?.FullName ?? User.FindFirstValue(ClaimTypes.Name) ?? "Mohammed Zakee";
        var resolvedEmail = user?.Email ?? User.FindFirstValue(ClaimTypes.Email) ?? "IT24104023@my.sliit.lk";
        var resolvedRole = user?.Role ?? User.FindFirstValue(ClaimTypes.Role) ?? "Administrator";
        var permissions = GetPermissionsForRole(resolvedRole);

        return Ok(new
        {
            id = user?.Id ?? Guid.NewGuid(),
            fullName = resolvedFullName,
            email = resolvedEmail,
            role = resolvedRole,
            phoneNumber = "+94 77 123 4567",
            department = "HSE Industrial Safety & Operations",
            bio = "Certified Lead Safety Officer and ClearToWork AI Permit-to-Work Systems Specialist.",
            avatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
            permissions = permissions
        });
    }

    [HttpPut("profile")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        User? user = null;
        if (Guid.TryParse(userIdStr, out var userId))
        {
            user = await _context.Users.FindAsync(userId);
        }

        if (user == null)
        {
            user = await _context.Users.FirstOrDefaultAsync();
        }

        if (user != null && !string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName.Trim();
            await _context.SaveChangesAsync();
        }

        var resolvedRole = user?.Role ?? User.FindFirstValue(ClaimTypes.Role) ?? "Administrator";
        var permissions = GetPermissionsForRole(resolvedRole);

        return Ok(new
        {
            id = user?.Id ?? Guid.NewGuid(),
            fullName = request.FullName ?? user?.FullName ?? "Mohammed Zakee",
            email = user?.Email ?? User.FindFirstValue(ClaimTypes.Email) ?? "IT24104023@my.sliit.lk",
            role = resolvedRole,
            phoneNumber = request.PhoneNumber ?? "+94 77 123 4567",
            department = request.Department ?? "HSE Industrial Safety & Operations",
            bio = request.Bio ?? "Certified Lead Safety Officer.",
            avatarUrl = request.AvatarUrl ?? "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
            permissions = permissions
        });
    }

    private static List<string> GetPermissionsForRole(string role)
    {
        return role switch
        {
            "Administrator" => new List<string>
            {
                "PERMIT_CREATE",
                "PERMIT_APPROVE",
                "PERMIT_REVOKE",
                "WORKFORCE_ADMIN",
                "EQUIPMENT_CALIBRATE",
                "SIMOPS_OVERRIDE",
                "AI_CLEARANCE_DISPATCH"
            },
            "SafetyOfficer" => new List<string>
            {
                "PERMIT_REVIEW",
                "PERMIT_SIGN_OFF",
                "EQUIPMENT_INSPECT",
                "ZONE_CONFLICT_AUDIT",
                "SAFETY_OBSERVATION_LOG"
            },
            "AreaSupervisor" => new List<string>
            {
                "PERMIT_SUBMIT",
                "WORKER_ASSIGN",
                "ISOLATION_TAG_EXECUTE",
                "ZONE_READINESS_CHECK"
            },
            _ => new List<string>
            {
                "PERMIT_REQUEST",
                "WORKER_ROSTER_VIEW",
                "COMPLIANCE_UPLOAD"
            }
        };
    }
}

public record UpdateUserProfileRequest(
    string? FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Department,
    string? Bio
);
