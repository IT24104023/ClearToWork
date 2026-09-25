using ClearToWork.Application.DTOs;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using ClearToWork.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClearToWork.Tests.Hazard;

/// <summary>
/// Unit and integration tests for <see cref="HazardRuleService"/> verifying SIMOPS clash detection,
/// hazard escalation matrices, and rulebook CRUD management.
/// </summary>
public class HazardRuleServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly HazardRuleService _hazardRuleService;

    // Test Fixture Data IDs
    private readonly Guid _siteId = Guid.NewGuid();
    private readonly Guid _zoneAlphaId = Guid.NewGuid();
    private readonly Guid _zoneBetaId = Guid.NewGuid();
    private readonly Guid _zoneGammaId = Guid.NewGuid(); // Isolated zone (not adjacent)
    private readonly Guid _hotWorkHazardId = Guid.NewGuid();
    private readonly Guid _solventHazardId = Guid.NewGuid();
    private readonly Guid _confinedSpaceHazardId = Guid.NewGuid();
    private readonly Guid _testUserId = Guid.NewGuid();

    public HazardRuleServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        SeedTestData();

        _hazardRuleService = new HazardRuleService(_context);
    }

    private void SeedTestData()
    {
        // 1. Site
        var site = new Site
        {
            Id = _siteId,
            Code = "OFFSHORE-01",
            Name = "Al-Shaheen Platform Alpha"
        };
        _context.Sites.Add(site);

        // 2. Zones
        var zoneAlpha = new Zone
        {
            Id = _zoneAlphaId,
            SiteId = _siteId,
            Code = "ZONE_A",
            Name = "Wellhead Deck Module A",
            Latitude = 24.5120m,
            Longitude = 54.3410m,
            RadiusMeters = 40.0,
            IsActive = true
        };

        var zoneBeta = new Zone
        {
            Id = _zoneBetaId,
            SiteId = _siteId,
            Code = "ZONE_B",
            Name = "Process Deck Module B",
            Latitude = 24.5125m,
            Longitude = 54.3415m,
            RadiusMeters = 45.0,
            IsActive = true
        };

        var zoneGamma = new Zone
        {
            Id = _zoneGammaId,
            SiteId = _siteId,
            Code = "ZONE_C",
            Name = "Helideck & Accommodation Module C",
            Latitude = 24.5200m,
            Longitude = 54.3500m,
            RadiusMeters = 50.0,
            IsActive = true
        };

        _context.Zones.AddRange(zoneAlpha, zoneBeta, zoneGamma);

        // 3. Adjacencies: Zone A is adjacent to Zone B (bidirectional)
        _context.ZoneAdjacencies.AddRange(
            new ZoneAdjacency { ZoneId = _zoneAlphaId, AdjacentZoneId = _zoneBetaId },
            new ZoneAdjacency { ZoneId = _zoneBetaId, AdjacentZoneId = _zoneAlphaId }
        );

        // 4. Hazard Types
        var hotWork = new HazardType
        {
            Id = _hotWorkHazardId,
            Code = "HOT_WORK",
            Name = "Hot Work (Welding / Flame Cutting)",
            SeverityLevel = HazardSeverity.High,
            MaxWindSpeedKmh = 35.0,
            ProhibitedInRain = false
        };

        var solventPainting = new HazardType
        {
            Id = _solventHazardId,
            Code = "SOLVENT_PAINTING",
            Name = "Solvent-based Spray Painting",
            SeverityLevel = HazardSeverity.Medium,
            MaxWindSpeedKmh = 25.0,
            ProhibitedInRain = true
        };

        var confinedSpace = new HazardType
        {
            Id = _confinedSpaceHazardId,
            Code = "CONFINED_SPACE",
            Name = "Confined Space Entry",
            SeverityLevel = HazardSeverity.Critical,
            ProhibitedInRain = false
        };

        _context.HazardTypes.AddRange(hotWork, solventPainting, confinedSpace);

        // 5. Permit Types
        var ptHotWork = new PermitType
        {
            Id = Guid.NewGuid(),
            Code = "HOT_WORK",
            Name = "Hot Work Permit",
            RequiresSafetyOfficerSignOff = true
        };

        var ptSolvent = new PermitType
        {
            Id = Guid.NewGuid(),
            Code = "SOLVENT_PAINTING",
            Name = "Solvent Coating Permit",
            RequiresSafetyOfficerSignOff = false
        };

        _context.PermitTypes.AddRange(ptHotWork, ptSolvent);

        // 6. Incompatibility Rule: HOT_WORK + SOLVENT_PAINTING clash across adjacent zones
        _context.IncompatibilityRules.Add(new IncompatibilityRule
        {
            Id = Guid.NewGuid(),
            RuleCode = "HR-07",
            PrimaryHazardId = _hotWorkHazardId,
            ConflictingHazardId = _solventHazardId,
            Reason = "Open ignition source (welding) in proximity to volatile solvent vapor cloud creates severe flash-fire risk.",
            AppliesToAdjacentZones = true
        });

        // 7. Seed Active Permit in Zone Beta (Solvent Painting from 09:00 to 13:00 tomorrow)
        var scheduledDay = DateTime.UtcNow.Date.AddDays(1);
        var activePermit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-2026-ACTIVE-01",
            ZoneId = _zoneBetaId,
            PermitTypeId = ptSolvent.Id,
            PermitType = ptSolvent,
            Status = PermitStatus.Active,
            ScheduledStartTime = scheduledDay.AddHours(9),
            ScheduledEndTime = scheduledDay.AddHours(13),
            Title = "Module B Structural Anti-Corrosion Spray Painting"
        };
        _context.PermitRequests.Add(activePermit);

        _context.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }

    [Fact]
    public async Task CheckZoneConflicts_WhenTwoActivitiesOverlapInAdjacentZone_DetectsSimopsClash()
    {
        // Arrange: Attempting to schedule HOT_WORK in Zone A during 10:00-12:00, overlapping active solvent painting in Zone B
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new ZoneConflictCheckRequest(
            ZoneId: _zoneAlphaId,
            HazardCode: "HOT_WORK",
            StartTime: tomorrow.AddHours(10),
            EndTime: tomorrow.AddHours(12)
        );

        // Act
        var result = await _hazardRuleService.CheckZoneConflictsAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.HasConflict);
        Assert.NotEmpty(result.Conflicts);

        var conflict = result.Conflicts.First();
        Assert.Equal("PTW-2026-ACTIVE-01", conflict.ConflictingPermitNumber);
        Assert.Equal("HR-07", conflict.ReasonRuleCode);
        Assert.Contains("volatile solvent vapor cloud", conflict.Explanation);
        Assert.NotNull(result.SuggestedAlternativeTimeWindow);
        Assert.Contains("after adjacent solvent painting permit completes", result.SuggestedAlternativeTimeWindow);
    }

    [Fact]
    public async Task CheckZoneConflicts_WhenActivitiesDoNotOverlapInTime_ReturnsNoConflict()
    {
        // Arrange: Scheduling HOT_WORK in Zone A at 14:00-16:00 (after solvent painting finishes at 13:00)
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new ZoneConflictCheckRequest(
            ZoneId: _zoneAlphaId,
            HazardCode: "HOT_WORK",
            StartTime: tomorrow.AddHours(14),
            EndTime: tomorrow.AddHours(16)
        );

        // Act
        var result = await _hazardRuleService.CheckZoneConflictsAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasConflict);
        Assert.Empty(result.Conflicts);
        Assert.Null(result.SuggestedAlternativeTimeWindow);
    }

    [Fact]
    public async Task CheckZoneConflicts_WhenNonAdjacentZones_ReturnsNoConflict()
    {
        // Arrange: Scheduling HOT_WORK in Zone C (Helideck, non-adjacent to Zone B) during same 10:00-12:00 window
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new ZoneConflictCheckRequest(
            ZoneId: _zoneGammaId,
            HazardCode: "HOT_WORK",
            StartTime: tomorrow.AddHours(10),
            EndTime: tomorrow.AddHours(12)
        );

        // Act
        var result = await _hazardRuleService.CheckZoneConflictsAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.HasConflict);
        Assert.Empty(result.Conflicts);
    }

    [Theory]
    [InlineData(HazardSeverity.Low, HazardSeverity.Low, HazardSeverity.Low)]
    [InlineData(HazardSeverity.Medium, HazardSeverity.Medium, HazardSeverity.High)]
    [InlineData(HazardSeverity.High, HazardSeverity.Medium, HazardSeverity.Critical)]
    [InlineData(HazardSeverity.Critical, HazardSeverity.High, HazardSeverity.Extreme)]
    [InlineData(HazardSeverity.Extreme, HazardSeverity.Critical, HazardSeverity.Extreme)]
    public void HazardSeverity_WhenMultipleHazardsCombine_EscalatesSeverityLevel(
        HazardSeverity primary,
        HazardSeverity secondary,
        HazardSeverity expectedEscalated)
    {
        // Act: Test escalation algorithm defined in HazardSeverityExtensions
        var calculated = primary.Escalate(secondary);

        // Assert
        Assert.Equal(expectedEscalated, calculated);
    }

    [Fact]
    public async Task HazardType_CrudOperations_CreateUpdateAndGetSuccessfully()
    {
        // 1. CREATE
        var createRequest = new CreateHazardTypeRequest(
            Code: "HYDROBLASTING",
            Name: "High Pressure Ultra-Hydroblasting (>1000 bar)",
            SeverityLevel: "Critical",
            MaxWindSpeedKmh: 40.0,
            ProhibitedInRain: true
        );

        var created = await _hazardRuleService.CreateHazardTypeAsync(createRequest);
        Assert.NotNull(created);
        Assert.Equal("HYDROBLASTING", created.Code);
        Assert.Equal("Critical", created.SeverityLevel);

        // 2. READ
        var retrieved = await _hazardRuleService.GetHazardTypeByIdAsync(created.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(created.Name, retrieved.Name);

        // 3. UPDATE
        var updateRequest = new UpdateHazardTypeRequest(
            Name: "Ultra High-Pressure Hydroblasting (>1500 bar)",
            SeverityLevel: "Critical",
            MaxWindSpeedKmh: 30.0,
            ProhibitedInRain: true
        );
        var updated = await _hazardRuleService.UpdateHazardTypeAsync(created.Id, updateRequest);
        Assert.NotNull(updated);
        Assert.Equal(30.0, updated.MaxWindSpeedKmh);
        Assert.Equal("Ultra High-Pressure Hydroblasting (>1500 bar)", updated.Name);

        // 4. DELETE
        var deleteResult = await _hazardRuleService.DeleteHazardTypeAsync(created.Id);
        Assert.True(deleteResult);

        var verifyDeleted = await _hazardRuleService.GetHazardTypeByIdAsync(created.Id);
        Assert.Null(verifyDeleted);
    }

    [Fact]
    public async Task IncompatibilityRule_CrudOperations_CreateUpdateAndDeleteSuccessfully()
    {
        // 1. CREATE Rule between Hot Work and Confined Space
        var createRequest = new CreateIncompatibilityRuleRequest(
            RuleCode: "HR-12",
            PrimaryHazardId: _hotWorkHazardId,
            ConflictingHazardId: _confinedSpaceHazardId,
            Reason: "Hot work inside or directly adjacent to unventilated confined spaces induces catastrophic toxic gas accumulation.",
            AppliesToAdjacentZones: false
        );

        var createdRule = await _hazardRuleService.CreateIncompatibilityRuleAsync(createRequest);
        Assert.NotNull(createdRule);
        Assert.Equal("HR-12", createdRule.RuleCode);
        Assert.Equal("HOT_WORK", createdRule.PrimaryHazardCode);

        // 2. READ ALL
        var rules = await _hazardRuleService.GetIncompatibilityRulesAsync();
        Assert.Contains(rules, r => r.RuleCode == "HR-12");

        // 3. UPDATE Rule
        var updateRequest = new UpdateIncompatibilityRuleRequest(
            Reason: "Hot work within 15 meters of confined space access portal requires continuous forced draft extraction.",
            AppliesToAdjacentZones: true
        );
        var updatedRule = await _hazardRuleService.UpdateIncompatibilityRuleAsync(createdRule.Id, updateRequest);
        Assert.NotNull(updatedRule);
        Assert.True(updatedRule.AppliesToAdjacentZones);
        Assert.Contains("15 meters", updatedRule.Reason);

        // 4. DELETE Rule
        var deleted = await _hazardRuleService.DeleteIncompatibilityRuleAsync(createdRule.Id);
        Assert.True(deleted);

        var rulesAfterDelete = await _hazardRuleService.GetIncompatibilityRulesAsync();
        Assert.DoesNotContain(rulesAfterDelete, r => r.RuleCode == "HR-12");
    }

    [Fact]
    public async Task SafetyObservations_CreateAndFilterByZone_RetrievesZoneSpecificHazards()
    {
        // Arrange
        var obsRequest1 = new CreateObservationRequest(
            ZoneId: _zoneAlphaId,
            Category: "GasLeakMinor",
            Description: "Flange joint FL-402 weeping trace condensates; LEL reading is 4%."
        );
        var obsRequest2 = new CreateObservationRequest(
            ZoneId: _zoneBetaId,
            Category: "Housekeeping",
            Description: "Loose scaffolding boards near east escape stairway."
        );

        // Act
        var obs1 = await _hazardRuleService.CreateObservationAsync(_testUserId, obsRequest1);
        var obs2 = await _hazardRuleService.CreateObservationAsync(_testUserId, obsRequest2);

        var zoneAObservations = await _hazardRuleService.GetObservationsAsync(zoneId: _zoneAlphaId);
        var zoneBObservations = await _hazardRuleService.GetObservationsAsync(zoneId: _zoneBetaId);

        // Assert
        Assert.NotNull(obs1);
        Assert.NotNull(obs2);
        Assert.Contains(zoneAObservations, o => o.Id == obs1.Id);
        Assert.DoesNotContain(zoneAObservations, o => o.Id == obs2.Id);

        Assert.Contains(zoneBObservations, o => o.Id == obs2.Id);
        Assert.DoesNotContain(zoneBObservations, o => o.Id == obs1.Id);
    }

    [Fact]
    public async Task SimopsMatrix_GeneratesIncompatibilitySummaryForFacility()
    {
        // Arrange: Fetch all rules and ensure active rules form a consistent matrix
        var rules = await _hazardRuleService.GetIncompatibilityRulesAsync();
        var allHazards = await _hazardRuleService.GetHazardTypesAsync();

        // Act & Assert
        Assert.NotEmpty(rules);
        Assert.NotEmpty(allHazards);

        // Verify HR-07 rule reciprocity and hazard bindings
        var hr07 = rules.FirstOrDefault(r => r.RuleCode == "HR-07");
        Assert.NotNull(hr07);
        Assert.True(hr07.AppliesToAdjacentZones);
        Assert.Equal("HOT_WORK", hr07.PrimaryHazardCode);
        Assert.Equal("SOLVENT_PAINTING", hr07.ConflictingHazardCode);
    }
}
