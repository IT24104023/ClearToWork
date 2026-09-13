using ClearToWork.Application.DTOs;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using ClearToWork.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearToWork.UnitTests;

public class WorkforceAndEquipmentTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly WorkforceService _workforceService;
    private readonly EquipmentService _equipmentService;
    private readonly HazardRuleService _hazardService;

    public WorkforceAndEquipmentTests()
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
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }

    [Fact]
    public async Task CheckEligibility_WithExpiredWorker_IdentifiesNonCompetencyAndAlternative()
    {
        var expiredWelder = await _context.Workers.FirstAsync(w => w.BadgeNumber == "W-1182");

        var response = await _workforceService.CheckEligibilityAsync(new EligibilityCheckRequest(
            new List<Guid> { expiredWelder.Id },
            "HOT_WORK",
            DateTime.UtcNow
        ));

        Assert.False(response.AllEligible);
        Assert.Single(response.Results);
        Assert.False(response.Results[0].IsEligible);
        Assert.NotEmpty(response.RecommendedReplacements);
        Assert.Contains(response.RecommendedReplacements, a => a.BadgeNumber == "W-1204");
    }

    [Fact]
    public async Task Get30DayExpiryForecast_ReturnsExpiringCertificates()
    {
        var forecast = await _workforceService.Get30DayExpiryForecastAsync();
        Assert.NotNull(forecast);
        Assert.Contains(forecast, f => f.BadgeNumber == "W-1182");
    }

    [Fact]
    public async Task EquipmentReservation_PreventsDoubleBooking_InsideTransaction()
    {
        var extinguisher = await _context.Assets.FirstAsync(a => a.AssetTag == "EX-31");
        var startTime = DateTime.UtcNow.Date.AddDays(2).AddHours(8);
        var endTime = DateTime.UtcNow.Date.AddDays(2).AddHours(12);

        // Pre-create permit 1
        var permit1 = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-RSV-001",
            PermitTypeId = (await _context.PermitTypes.FirstAsync()).Id,
            ZoneId = (await _context.Zones.FirstAsync()).Id,
            Status = PermitStatus.Approved,
            ScheduledStartTime = startTime,
            ScheduledEndTime = endTime
        };
        _context.PermitRequests.Add(permit1);

        // Pre-create permit 2
        var permit2 = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-RSV-002",
            PermitTypeId = (await _context.PermitTypes.FirstAsync()).Id,
            ZoneId = (await _context.Zones.FirstAsync()).Id,
            Status = PermitStatus.PendingApproval,
            ScheduledStartTime = startTime.AddHours(1),
            ScheduledEndTime = endTime.AddHours(2)
        };
        _context.PermitRequests.Add(permit2);
        await _context.SaveChangesAsync();

        // First reservation succeeds
        var firstResult = await _equipmentService.ReserveEquipmentTransactionAsync(
            permit1.Id,
            new List<Guid> { extinguisher.Id },
            startTime,
            endTime
        );
        Assert.True(firstResult);

        // Second overlapping reservation must fail
        var secondResult = await _equipmentService.ReserveEquipmentTransactionAsync(
            permit2.Id,
            new List<Guid> { extinguisher.Id },
            startTime.AddHours(1),
            endTime.AddHours(2)
        );
        Assert.False(secondResult);
    }

    [Fact]
    public async Task ZoneConflictCheck_DetectsAdjacentIncompatibleHazard()
    {
        var zoneB3 = await _context.Zones.FirstAsync(z => z.Code == "ZONE_B3");

        // Request hot work in Zone B3 during the time window of PTW-2026-0403 (09:00 - 12:00 tomorrow in adjacent Zone B4)
        var response = await _hazardService.CheckZoneConflictsAsync(new ZoneConflictCheckRequest(
            zoneB3.Id,
            "HOT_WORK",
            DateTime.UtcNow.Date.AddDays(1).AddHours(9),
            DateTime.UtcNow.Date.AddDays(1).AddHours(11)
        ));

        Assert.True(response.HasConflict);
        Assert.NotEmpty(response.Conflicts);
        Assert.Contains(response.Conflicts, c => c.ReasonRuleCode == "HR-07");
        Assert.NotNull(response.SuggestedAlternativeTimeWindow);
    }
}
