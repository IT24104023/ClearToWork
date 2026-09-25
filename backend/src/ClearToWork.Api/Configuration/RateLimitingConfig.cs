using System.Net;
using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace ClearToWork.Api.Configuration;

/// <summary>
/// Constants representing rate limiting policy names across ClearToWork AI API.
/// </summary>
public static class RateLimitPolicies
{
    /// <summary>
    /// General query and read policy: 100 requests per minute.
    /// </summary>
    public const string General = "general";

    /// <summary>
    /// Permit mutations policy: 20 requests per minute for creates, updates, and workflow reviews.
    /// </summary>
    public const string PermitMutations = "permit-mutations";

    /// <summary>
    /// Auth endpoints policy: 10 requests per minute for login and credential validation (brute-force defense).
    /// </summary>
    public const string AuthEndpoints = "auth-endpoints";
}

/// <summary>
/// Configures ASP.NET Core rate limiting using <see cref="System.Threading.RateLimiting"/>
/// to protect critical safety management endpoints against abuse and denial-of-service.
/// </summary>
public static class RateLimitingConfig
{
    /// <summary>
    /// Registers fixed-window rate limiting policies tailored to ClearToWork AI operational tiers.
    /// </summary>
    /// <param name="services">The DI service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global 429 Too Many Requests response formatting
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

                var retryAfter = "60";
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retrySpan))
                {
                    retryAfter = ((int)retrySpan.TotalSeconds).ToString();
                    context.HttpContext.Response.Headers.Append("Retry-After", retryAfter);
                }

                var problemDetails = new
                {
                    status = StatusCodes.Status429TooManyRequests,
                    title = "Too Many Requests",
                    detail = $"Rate limit quota reached for this endpoint. Please retry after {retryAfter} seconds.",
                    instance = context.HttpContext.Request.Path.Value,
                    timestamp = DateTime.UtcNow
                };

                await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(problemDetails),
                    cancellationToken);
            };

            // 1. General endpoints: 100 req/min with modest buffer queue
            options.AddFixedWindowLimiter(policyName: RateLimitPolicies.General, opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 10;
            });

            // 2. Permit mutations (create draft, update, submit, activate): 20 req/min
            options.AddFixedWindowLimiter(policyName: RateLimitPolicies.PermitMutations, opt =>
            {
                opt.PermitLimit = 20;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 5;
            });

            // 3. Auth endpoints (login, register): 10 req/min with zero queueing to counter brute-force attacks
            options.AddFixedWindowLimiter(policyName: RateLimitPolicies.AuthEndpoints, opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
        });

        return services;
    }

    /// <summary>
    /// Enables rate limiting middleware in the ASP.NET Core request pipeline.
    /// </summary>
    public static IApplicationBuilder UseApiRateLimiting(this IApplicationBuilder app)
    {
        return app.UseRateLimiter();
    }
}
