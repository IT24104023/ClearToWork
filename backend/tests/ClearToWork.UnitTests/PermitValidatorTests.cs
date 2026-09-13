using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using ClearToWork.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearToWork.UnitTests;

public class PermitValidatorTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly IWorkforceService _workforceService;
    private readonly IEquipmentService _equipmentService;
    private readonly IHazardRuleService _hazardService;
    private readonly MockWeatherService _weatherService;
    private readonly DeterministicPermitValidator _validator;

    public PermitValidatorTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        DbInitializer.SeedAsync(_context).GetAwaiter().GetResult();

        _workforceService = new WorkforceService(_context);
        _equipmentService = new EquipmentService(_context);
        _hazardService = new HazardRuleService(_context);
        _weatherService = new MockWeatherService();

        _validator = new DeterministicPermitValidator(
            _context,
            _workforceService,
            _equipmentService,
            _hazardService,
            _weatherService
        );
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }

    [Fact]
    public async Task GoldenCase1_CleanPermit_ShouldBeApprovedWithClearVerdict()
    {
        // Arrange: Valid welder (W-1204), Valid extinguisher (EX-31), Safe time window without conflict (tomorrow 14:00-16:00), Calm weather
        var zoneB3 = await _context.Zones.FirstAsync(z => z.Code == "ZONE_B3");
        var hotWorkType = await _context.PermitTypes.FirstAsync(p => p.Code == "HOT_WORK");
        var validWelder = await _context.Workers.FirstAsync(w => w.BadgeNumber == "W-1204");
        var validExtinguisher = await _context.Assets.FirstAsync(a => a.AssetTag == "EX-31");

        _weatherService.ConfiguredGusts = 18.0; // Calm wind

        var permit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-TEST-001",
            ZoneId = zoneB3.Id,
            PermitTypeId = hotWorkType.Id,
            ScheduledStartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(14), // After conflicting permit completes
            ScheduledEndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            Status = PermitStatus.Submitted
        };

        permit.AssignedWorkers.Add(new PermitWorker { PermitRequestId = permit.Id, WorkerId = validWelder.Id });
        permit.AssignedAssets.Add(new PermitAsset { PermitRequestId = permit.Id, AssetId = validExtinguisher.Id });

        // Act
        var report = await _validator.ValidatePermitRulesAsync(permit);

        // Assert
        Assert.True(report.IsApproved);
        Assert.Equal("CLEAR", report.Verdict);
        Assert.Empty(report.HardFailureReasons);
    }

    [Fact]
    public async Task GoldenCase2_ExpiredCertificate_ShouldRefuseAndNameSpecificWorker()
    {
        // Arrange: Expired welder (W-1182)
        var zoneB3 = await _context.Zones.FirstAsync(z => z.Code == "ZONE_B3");
        var hotWorkType = await _context.PermitTypes.FirstAsync(p => p.Code == "HOT_WORK");
        var expiredWelder = await _context.Workers.FirstAsync(w => w.BadgeNumber == "W-1182");
        var validExtinguisher = await _context.Assets.FirstAsync(a => a.AssetTag == "EX-31");

        var permit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-TEST-002",
            ZoneId = zoneB3.Id,
            PermitTypeId = hotWorkType.Id,
            ScheduledStartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(14),
            ScheduledEndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            Status = PermitStatus.Submitted
        };

        permit.AssignedWorkers.Add(new PermitWorker { PermitRequestId = permit.Id, WorkerId = expiredWelder.Id });
        permit.AssignedAssets.Add(new PermitAsset { PermitRequestId = permit.Id, AssetId = validExtinguisher.Id });

        // Act
        var report = await _validator.ValidatePermitRulesAsync(permit);

        // Assert
        Assert.False(report.IsApproved);
        Assert.Equal("REFUSED_SAFE_FAILURE", report.Verdict);
        Assert.Contains(report.HardFailureReasons, f => f.Contains("W-1182") && f.Contains("expired"));
        Assert.NotNull(report.ProposedFix);
        Assert.Contains("W-1204", report.ProposedFix.SuggestedWorkerBadge);
    }

    [Fact]
    public async Task GoldenCase3_OverdueEquipmentInspection_ShouldRefuseAndDetailInspection()
    {
        // Arrange: Valid welder, but Overdue fire extinguisher (EX-22)
        var zoneB3 = await _context.Zones.FirstAsync(z => z.Code == "ZONE_B3");
        var hotWorkType = await _context.PermitTypes.FirstAsync(p => p.Code == "HOT_WORK");
        var validWelder = await _context.Workers.FirstAsync(w => w.BadgeNumber == "W-1204");
        var overdueExtinguisher = await _context.Assets.FirstAsync(a => a.AssetTag == "EX-22");

        var permit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-TEST-003",
            ZoneId = zoneB3.Id,
            PermitTypeId = hotWorkType.Id,
            ScheduledStartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(14),
            ScheduledEndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            Status = PermitStatus.Submitted
        };

        permit.AssignedWorkers.Add(new PermitWorker { PermitRequestId = permit.Id, WorkerId = validWelder.Id });
        permit.AssignedAssets.Add(new PermitAsset { PermitRequestId = permit.Id, AssetId = overdueExtinguisher.Id });

        // Act
        var report = await _validator.ValidatePermitRulesAsync(permit);

        // Assert
        Assert.False(report.IsApproved);
        Assert.Contains(report.HardFailureReasons, f => f.Contains("EX-22") && f.Contains("inspection overdue"));
    }

    [Fact]
    public async Task GoldenCase4_AdjacentZoneConflict_ShouldRefuseWithRuleHR07()
    {
        // Arrange: Hot work scheduled during 09:00-11:00 in Zone B3 overlapping with active solvent painting in Zone B4
        var zoneB3 = await _context.Zones.FirstAsync(z => z.Code == "ZONE_B3");
        var hotWorkType = await _context.PermitTypes.FirstAsync(p => p.Code == "HOT_WORK");
        var validWelder = await _context.Workers.FirstAsync(w => w.BadgeNumber == "W-1204");
        var validExtinguisher = await _context.Assets.FirstAsync(a => a.AssetTag == "EX-31");

        _weatherService.ConfiguredGusts = 20.0;

        var permit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-TEST-004",
            ZoneId = zoneB3.Id,
            PermitTypeId = hotWorkType.Id,
            ScheduledStartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9), // Clashes with PTW-2026-0403 (09:00-12:00)
            ScheduledEndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(11),
            Status = PermitStatus.Submitted
        };

        permit.AssignedWorkers.Add(new PermitWorker { PermitRequestId = permit.Id, WorkerId = validWelder.Id });
        permit.AssignedAssets.Add(new PermitAsset { PermitRequestId = permit.Id, AssetId = validExtinguisher.Id });

        // Act
        var report = await _validator.ValidatePermitRulesAsync(permit);

        // Assert
        Assert.False(report.IsApproved);
        Assert.Contains(report.HardFailureReasons, f => f.Contains("Zone clash") && f.Contains("PTW-2026-0403"));
    }

    [Fact]
    public async Task GoldenCase5_AdverseWeather_ExcessiveWindGusts_ShouldRefuse()
    {
        // Arrange: Hot work when wind gusts are 44 km/h (exceeding 35 km/h limit)
        var zoneB3 = await _context.Zones.FirstAsync(z => z.Code == "ZONE_B3");
        var hotWorkType = await _context.PermitTypes.FirstAsync(p => p.Code == "HOT_WORK");
        var validWelder = await _context.Workers.FirstAsync(w => w.BadgeNumber == "W-1204");
        var validExtinguisher = await _context.Assets.FirstAsync(a => a.AssetTag == "EX-31");

        _weatherService.ConfiguredGusts = 44.0; // Exceeds 35 km/h limit

        var permit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-TEST-005",
            ZoneId = zoneB3.Id,
            PermitTypeId = hotWorkType.Id,
            ScheduledStartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(14),
            ScheduledEndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            Status = PermitStatus.Submitted
        };

        permit.AssignedWorkers.Add(new PermitWorker { PermitRequestId = permit.Id, WorkerId = validWelder.Id });
        permit.AssignedAssets.Add(new PermitAsset { PermitRequestId = permit.Id, AssetId = validExtinguisher.Id });

        // Act
        var report = await _validator.ValidatePermitRulesAsync(permit);

        // Assert
        Assert.False(report.IsApproved);
        Assert.Contains(report.HardFailureReasons, f => f.Contains("44 km/h") && f.Contains("exceed the 35 km/h limit"));
    }

    [Fact]
    public async Task GoldenCase6_NoWorkersAssigned_ShouldRefuse()
    {
        // Arrange: Permit without assigned workers
        var zoneB3 = await _context.Zones.FirstAsync(z => z.Code == "ZONE_B3");
        var hotWorkType = await _context.PermitTypes.FirstAsync(p => p.Code == "HOT_WORK");

        var permit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-TEST-006",
            ZoneId = zoneB3.Id,
            PermitTypeId = hotWorkType.Id,
            ScheduledStartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(14),
            ScheduledEndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(16),
            Status = PermitStatus.Submitted
        };

        // Act
        var report = await _validator.ValidatePermitRulesAsync(permit);

        // Assert
        Assert.False(report.IsApproved);
        Assert.Contains(report.HardFailureReasons, f => f.Contains("No qualified workers"));
    }
}

public class MockWeatherService : IWeatherService
{
    public double ConfiguredGusts { get; set; } = 15.0;

    public Task<WeatherForecastDto> GetForecastAsync(decimal latitude, decimal longitude, DateTime targetTime)
    {
        return Task.FromResult(new WeatherForecastDto(
            latitude,
            longitude,
            TemperatureC: 26.0,
            WindSpeedKmh: 12.0,
            WindGustsKmh: ConfiguredGusts,
            PrecipitationProbability: 10.0,
            IsRainExpected: false,
            IsSafeForHotWork: ConfiguredGusts <= 35.0,
            IsSafeForHeightWork: ConfiguredGusts <= 30.0,
            Summary: $"Wind gusts {ConfiguredGusts} km/h"
        ));
    }
}
