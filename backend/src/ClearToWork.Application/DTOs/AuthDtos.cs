using ClearToWork.Domain.Enums;

namespace ClearToWork.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string Token,
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    Guid? ContractorId
);

public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    Guid? ContractorId
);
