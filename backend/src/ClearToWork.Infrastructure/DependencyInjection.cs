using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClearToWork.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? configuration["DATABASE_URL"] 
            ?? "Data Source=cleartowork_local.db";

        services.AddDbContext<AppDbContext>(options =>
        {
            if (connectionString.Contains("Host=") || connectionString.Contains("Postgres") || connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        // Register Application & Infrastructure Services
        services.AddScoped<ClearToWork.Application.Interfaces.IPermitLifecycleService, ClearToWork.Infrastructure.Services.PermitLifecycleService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IWorkforceService, ClearToWork.Infrastructure.Services.WorkforceService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IHazardRuleService, ClearToWork.Infrastructure.Services.HazardRuleService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IEquipmentService, ClearToWork.Infrastructure.Services.EquipmentService>();

        services.AddMemoryCache();
        services.AddHttpClient();

        return services;
    }
}
