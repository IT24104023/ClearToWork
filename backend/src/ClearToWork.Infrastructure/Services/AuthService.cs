using System;
using System.Threading.Tasks;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;

namespace ClearToWork.Infrastructure.Services;

public class AuthService : IAuthService
{
    public Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = new AuthResponse(
            Token: "dummy-jwt-token-cleartowork-2026",
            UserId: Guid.NewGuid(),
            FullName: "Mohammed Zakee",
            Email: request.Email,
            Role: "SafetyOfficer",
            ContractorId: null
        );
        return Task.FromResult<AuthResponse?>(response);
    }

    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = new AuthResponse(
            Token: "dummy-jwt-token-cleartowork-2026",
            UserId: Guid.NewGuid(),
            FullName: request.FullName,
            Email: request.Email,
            Role: request.Role ?? "Worker",
            ContractorId: request.ContractorId
        );
        return Task.FromResult(response);
    }
}
