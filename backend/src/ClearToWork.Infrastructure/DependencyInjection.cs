using ClearToWork.Application.Interfaces;
using ClearToWork.Infrastructure.Data;
using ClearToWork.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClearToWork.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. EF Core Database Setup (PostgreSQL with SQLite fallback for local developer ergonomics)
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            if (string.IsNullOrWhiteSpace(connectionString) || 
                connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase) || 
                connectionString.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
            {
                var sqliteConn = string.IsNullOrWhiteSpace(connectionString) ? "Data Source=cleartowork_local.db" : connectionString;
                options.UseSqlite(sqliteConn);
            }
            else
            {
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            }
        });

        // 2. Memory Cache for Weather and Metadata
        services.AddMemoryCache();

        // 3. Resilient HTTP Clients (Open-Meteo & Agent Microservice)
        services.AddHttpClient();
        services.AddHttpClient<IWeatherService, OpenMeteoWeatherService>();

        // 4. Register Services for DI
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWorkforceService, WorkforceService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IHazardRuleService, HazardRuleService>();
        services.AddScoped<IPermitValidator, DeterministicPermitValidator>();
        services.AddScoped<IPermitLifecycleService, PermitLifecycleService>();

        return services;
    }
}
