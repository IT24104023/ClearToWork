using ClearToWork.Domain.Enums;

namespace ClearToWork.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string Token,
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    Guid? ContractorId,
    string? AvatarUrl = null
);

public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    Guid? ContractorId
);

public record UserProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    Guid? ContractorId,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Department,
    string? Bio,
    List<string> Permissions
);

public record UpdateProfileRequest(
    string FullName,
    string? AvatarUrl,
    string? PhoneNumber,
    string? Department,
    string? Bio
);

