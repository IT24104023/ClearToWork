using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClearToWork.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Task.FromResult<AuthResponse?>(null);
        }

        var (role, fullName) = request.Email.ToLowerInvariant() switch
        {
            "safety@cleartowork.com" => ("SafetyOfficer", "Elena Rostova"),
            "supervisor@contractor.com" => ("Supervisor", "David Miller"),
            "areasup@cleartowork.com" => ("AreaSupervisor", "James Whitfield"),
            "admin@cleartowork.com" => ("Administrator", "System Administrator"),
            _ => ("SafetyOfficer", "Mohammed Zakee")
        };

        var userId = Guid.NewGuid();
        var jwtKey = _configuration["Jwt:Key"] ?? "ClearToWork_Super_Secret_Key_For_Development_Must_Be_32_Chars_Long!";
        var issuer = _configuration["Jwt:Issuer"] ?? "ClearToWorkAPI";
        var audience = _configuration["Jwt:Audience"] ?? "ClearToWorkClients";

        var claimsList = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, fullName),
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Role, role)
        };
        if (role == "Administrator") claimsList.Add(new Claim(ClaimTypes.Role, "Admin"));
        if (role == "Admin") claimsList.Add(new Claim(ClaimTypes.Role, "Administrator"));

        var claims = claimsList.ToArray();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new AuthResponse(
            Token: tokenString,
            UserId: userId,
            FullName: fullName,
            Email: request.Email,
            Role: role,
            ContractorId: role == "Supervisor" ? Guid.NewGuid() : null
        );

        return Task.FromResult<AuthResponse?>(response);
    }

    public Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var userId = Guid.NewGuid();
        var jwtKey = _configuration["Jwt:Key"] ?? "ClearToWork_Super_Secret_Key_For_Development_Must_Be_32_Chars_Long!";
        var issuer = _configuration["Jwt:Issuer"] ?? "ClearToWorkAPI";
        var audience = _configuration["Jwt:Audience"] ?? "ClearToWorkClients";
        var role = request.Role ?? "Worker";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, request.FullName),
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new AuthResponse(
            Token: tokenString,
            UserId: userId,
            FullName: request.FullName,
            Email: request.Email,
            Role: role,
            ContractorId: request.ContractorId
        );

        return Task.FromResult(response);
    }
}
