using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ClearToWork.Api.Middleware;

/// <summary>
/// ASP.NET Core middleware that checks JWT token expiry and automatically refreshes tokens
/// within a sliding window (e.g., 5 minutes before expiry) to keep active user sessions seamless.
/// Appends the refreshed token to the response headers and logs all authentication lifecycle events.
/// </summary>
public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtRefreshMiddleware> _logger;
    private readonly IConfiguration _configuration;

    // Token configuration constants
    public const string DefaultIssuer = "ClearToWorkAPI";
    public const string DefaultAudience = "ClearToWorkClients";
    public const int DefaultExpiryMinutes = 60;
    public const int DefaultRefreshWindowMinutes = 5;

    // Response header constants
    public const string RefreshTokenHeader = "X-Refreshed-Token";
    public const string TokenExpiresAtHeader = "X-Token-Expires-At";

    public JwtRefreshMiddleware(
        RequestDelegate next,
        ILogger<JwtRefreshMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Evaluates the request's Authorization header and refreshes expiring JWT tokens.
    /// </summary>
    /// <param name="context">The HTTP request context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var rawToken = authHeader["Bearer ".Length..].Trim();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(rawToken))
                {
                    var jwtToken = handler.ReadJwtToken(rawToken);
                    var expiryUtc = jwtToken.ValidTo;
                    var timeRemaining = expiryUtc - DateTime.UtcNow;

                    var refreshWindowMinutes = _configuration.GetValue("Jwt:RefreshWindowMinutes", DefaultRefreshWindowMinutes);
                    var refreshWindow = TimeSpan.FromMinutes(refreshWindowMinutes);

                    // Case 1: Valid token falling within sliding refresh window
                    if (timeRemaining > TimeSpan.Zero && timeRemaining <= refreshWindow)
                    {
                        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value ?? "Unknown";
                        var userEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "Unknown";
                        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Unknown";

                        _logger.LogInformation(
                            "[Auth] JWT sliding refresh window entered for User {UserId} ({Email}, Role: {Role}). Remaining: {Minutes:F1}m. Issuing refreshed token.",
                            userId, userEmail, role, timeRemaining.TotalMinutes);

                        var (refreshedToken, newExpiry) = GenerateRefreshedToken(jwtToken.Claims);

                        context.Response.OnStarting(() =>
                        {
                            if (!context.Response.Headers.ContainsKey(RefreshTokenHeader))
                            {
                                context.Response.Headers.Append(RefreshTokenHeader, refreshedToken);
                                context.Response.Headers.Append(TokenExpiresAtHeader, newExpiry.ToString("o"));
                                context.Response.Headers.Append("Access-Control-Expose-Headers", $"{RefreshTokenHeader}, {TokenExpiresAtHeader}");
                            }
                            return Task.CompletedTask;
                        });

                        _logger.LogDebug("[Auth] Refreshed token attached to response header for User {UserId}.", userId);
                    }
                    // Case 2: Token has already expired
                    else if (timeRemaining <= TimeSpan.Zero)
                    {
                        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                        _logger.LogWarning(
                            "[Auth] Expired JWT presented by User {UserId}. Expired at UTC {Expiry} ({MinutesAgo:F1}m ago).",
                            userId, expiryUtc, Math.Abs(timeRemaining.TotalMinutes));
                    }
                }
            }
            catch (Exception ex)
            {
                // Non-fatal: do not block the pipeline if header inspection encounters an issue
                _logger.LogWarning(ex, "[Auth] Exception occurred while inspecting JWT token in sliding refresh middleware.");
            }
        }

        await _next(context);
    }

    /// <summary>
    /// Constructs a refreshed JWT token preserving existing user identity claims with an extended validity period.
    /// </summary>
    private (string Token, DateTime ExpiryUtc) GenerateRefreshedToken(IEnumerable<Claim> existingClaims)
    {
        var keyStr = _configuration["Jwt:Key"] ?? "ClearToWork_Super_Secret_Key_For_Development_Must_Be_32_Chars_Long!";
        var issuer = _configuration["Jwt:Issuer"] ?? DefaultIssuer;
        var audience = _configuration["Jwt:Audience"] ?? DefaultAudience;
        var expiryMinutes = _configuration.GetValue("Jwt:ExpiryMinutes", DefaultExpiryMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Filter out token lifetime claims so they can be regenerated cleanly
        var preservedClaims = existingClaims.Where(c =>
            c.Type != JwtRegisteredClaimNames.Exp &&
            c.Type != JwtRegisteredClaimNames.Nbf &&
            c.Type != JwtRegisteredClaimNames.Iat &&
            c.Type != JwtRegisteredClaimNames.Iss &&
            c.Type != JwtRegisteredClaimNames.Aud).ToList();

        // Refresh JTI for uniqueness tracking
        preservedClaims.RemoveAll(c => c.Type == JwtRegisteredClaimNames.Jti);
        preservedClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        var newExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: preservedClaims,
            notBefore: DateTime.UtcNow,
            expires: newExpiry,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (tokenString, newExpiry);
    }
}

/// <summary>
/// Pipeline extension methods for registering <see cref="JwtRefreshMiddleware"/>.
/// </summary>
public static class JwtRefreshMiddlewareExtensions
{
    /// <summary>
    /// Adds the JWT sliding window refresh middleware to the application pipeline.
    /// </summary>
    public static IApplicationBuilder UseJwtRefresh(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtRefreshMiddleware>();
    }
}
