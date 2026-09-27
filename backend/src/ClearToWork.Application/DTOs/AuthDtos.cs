using System;

namespace ClearToWork.Application.DTOs;

public record LoginRequest(
    string Email,
    string Password
);

public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    string Role = "Worker",
    string? BadgeNumber = null,
    Guid? ContractorId = null
);

public record AuthResponse(
    string Token,
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    Guid? ContractorId
);
