using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Services;

namespace ClearToWork.Infrastructure;

/// <summary>
/// Dependency Injection container registration extension for ClearToWork Infrastructure layer.
/// Registers EF Core DbContext, HTTP clients, memory cache, and all specialized safety domain services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure persistence, EF Core DbContext, and safety domain services in the DI container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    /// <param name="configuration">Application configuration instance.</param>
    /// <returns>Updated <see cref="IServiceCollection"/>.</returns>
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

        // Register Primary Application & Infrastructure Services
        services.AddScoped<ClearToWork.Application.Interfaces.IPermitLifecycleService, ClearToWork.Infrastructure.Services.PermitLifecycleService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IWorkforceService, ClearToWork.Infrastructure.Services.WorkforceService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IHazardRuleService, ClearToWork.Infrastructure.Services.HazardRuleService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IEquipmentService, ClearToWork.Infrastructure.Services.EquipmentService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IPermitValidator, ClearToWork.Infrastructure.Services.PermitApprovalValidator>();
        services.AddScoped<ClearToWork.Infrastructure.Services.IPermitApprovalValidator, ClearToWork.Infrastructure.Services.PermitApprovalValidator>();
        services.AddScoped<ClearToWork.Application.Interfaces.IAuthService, ClearToWork.Infrastructure.Services.AuthService>();
        services.AddScoped<ClearToWork.Application.Interfaces.IWeatherService, ClearToWork.Infrastructure.Services.WeatherService>();

        // Register Specialized Industrial Safety Domain Services
        services.AddScoped<ISimultaneousOperationRiskScorer, SimultaneousOperationRiskScorer>();
        services.AddScoped<ITradeQualificationValidator, TradeQualificationValidator>();
        services.AddScoped<IExclusionZoneEmergencyEvacuationRoute, ExclusionZoneEmergencyEvacuationRoute>();
        services.AddScoped<IEmergencyResponseTeamRoleAssigner, EmergencyResponseTeamRoleAssigner>();
        services.AddScoped<ICompetencyVerificationAuditLog, CompetencyVerificationAuditLog>();
        services.AddScoped<IEquipmentMaintenanceHistoryLog, EquipmentMaintenanceHistoryLog>();
        services.AddScoped<IConfinedSpaceAtmosphereMonitoringSchedule, ConfinedSpaceAtmosphereMonitoringSchedule>();
        services.AddScoped<IIsolationCertificateLinker, IsolationCertificateLinker>();
        services.AddScoped<ISCBACylinderPressureChecker, SCBACylinderPressureChecker>();
        services.AddScoped<IBreathingAirCompressorAirQualityTester, BreathingAirCompressorAirQualityTester>();
        services.AddScoped<IPortableGasDetectorZeroPointCalibrator, PortableGasDetectorZeroPointCalibrator>();
        services.AddScoped<IQuarantineEquipmentWorkflow, QuarantineEquipmentWorkflow>();
        services.AddScoped<ILOTOComplianceChecker, LOTOComplianceChecker>();
        services.AddScoped<IGasReadingAnalyzer, GasReadingAnalyzer>();
        services.AddScoped<IEquipmentAnalyticsService, EquipmentAnalyticsService>();
        services.AddScoped<IWorkforceAnalyticsService, WorkforceAnalyticsService>();

        services.AddMemoryCache();
        services.AddHttpClient();

        return services;
    }
}