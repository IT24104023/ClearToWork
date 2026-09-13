using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClearToWork.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.IsActive);
        if (user == null) return null;

        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid) return null;

        var token = GenerateJwtToken(user);
        return new LoginResponse(
            token,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.ContractorId
        );
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (existing)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            ContractorId = request.ContractorId,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return new LoginResponse(
            token,
            user.Id,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.ContractorId
        );
    }

    private string GenerateJwtToken(User user)
    {
        var keyStr = _config["Jwt:Key"] ?? "ClearToWork_Super_Secret_Key_For_Development_Must_Be_32_Chars_Long!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.ContractorId.HasValue)
        {
            claims.Add(new Claim("ContractorId", user.ContractorId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "ClearToWorkAPI",
            audience: _config["Jwt:Audience"] ?? "ClearToWorkClients",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
