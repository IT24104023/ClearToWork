using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Identity;

public class User : BaseAuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "SafetyOfficer";
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
