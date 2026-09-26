using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.PermitTypes.AnyAsync())
        {
            return; // DB already seeded
        }

        // Seed Site & Zones
        var site = new Site
        {
            Id = Guid.NewGuid(),
            Name = "ClearToWork AI Industrial Refinery & Processing Plant",
            LocationCode = "PLANT-ALPHA-01",
            Latitude = 25.2048m,
            Longitude = 55.2708m
        };
        context.Sites.Add(site);

        var zoneA = new Zone
        {
            Id = Guid.NewGuid(),
            SiteId = site.Id,
            Code = "ZONE-A1",
            Name = "Crude Distillation Unit Deck",
            Latitude = 25.2048m,
            Longitude = 55.2708m,
            RadiusMeters = 50,
            QrCodePayload = "QR-ZONE-A1-CRUDE-DECK",
            IsActive = true
        };

        var zoneB = new Zone
        {
            Id = Guid.NewGuid(),
            SiteId = site.Id,
            Code = "ZONE-B2",
            Name = "Hydrocracker & Gas Storage Tank Farm",
            Latitude = 25.2055m,
            Longitude = 55.2715m,
            RadiusMeters = 75,
            QrCodePayload = "QR-ZONE-B2-HYDROCRACKER",
            IsActive = true
        };

        context.Zones.AddRange(zoneA, zoneB);

        // Zone Adjacency
        context.ZoneAdjacencies.Add(new ZoneAdjacency
        {
            ZoneId = zoneA.Id,
            AdjacentZoneId = zoneB.Id
        });

        // Seed Permit Types
        var hotWorkType = new PermitType
        {
            Id = Guid.NewGuid(),
            Code = "HOT_WORK",
            Name = "Hot Work Permit (Welding & Grinding)",
            Description = "Required for open flame, spark-producing, welding, cutting, or grinding activities.",
            MaxDurationHours = 8,
            RequiresFireWatch = true,
            RequiresGasTesting = true,
            RequiresIsolation = true,
            MandatoryControlsJson = "[\"Fire watch posted within 5m\", \"Continuous LEL/O2 gas monitoring\", \"Spark containment blankets\"]"
        };

        var coldWorkType = new PermitType
        {
            Id = Guid.NewGuid(),
            Code = "COLD_WORK",
            Name = "Cold Work Maintenance Permit",
            Description = "Required for mechanical assembly, piping flange tie-ins, and non-sparking inspection.",
            MaxDurationHours = 12,
            RequiresFireWatch = false,
            RequiresGasTesting = false,
            RequiresIsolation = false,
            MandatoryControlsJson = "[\"Standard PPE\", \"Safety harness if above 2m\"]"
        };

        context.PermitTypes.AddRange(hotWorkType, coldWorkType);

        // Seed Users & Contractor
        var contractor = new Contractor
        {
            Id = Guid.NewGuid(),
            CompanyName = "Global Energy Maintenance Corp",
            RegistrationNumber = "REG-2026-9901",
            SafetyRating = "A+"
        };
        context.Contractors.Add(contractor);

        var worker1 = new Worker
        {
            Id = Guid.NewGuid(),
            BadgeNumber = "W-101",
            FirstName = "Mohammed",
            LastName = "Zakee",
            Trade = "Certified Pipefitter & Lead Isolation Tech",
            ContractorId = contractor.Id,
            IsActive = true
        };

        var worker2 = new Worker
        {
            Id = Guid.NewGuid(),
            BadgeNumber = "W-102",
            FirstName = "Chemini",
            LastName = "Perera",
            Trade = "HSE Gas Safety Specialist",
            ContractorId = contractor.Id,
            IsActive = true
        };

        context.Workers.AddRange(worker1, worker2);

        // Seed Assets
        var gasMonitor = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = "GAS-MON-401",
            SerialNo = "SN-9981-H2S",
            Name = "Dräger X-am 5000 Multi-Gas Detector",
            Category = AssetCategory.GasDetector,
            Status = AssetStatus.Available,
            ZoneId = zoneA.Id
        };

        var isolationBreaker = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = "SWGR-02-BKR-14",
            SerialNo = "SN-7720-ELEC",
            Name = "Main High Voltage Circuit Breaker 4160V",
            Category = AssetCategory.IsolationDevice,
            Status = AssetStatus.Available,
            ZoneId = zoneA.Id
        };

        context.Assets.AddRange(gasMonitor, isolationBreaker);

        // Seed Initial Sample Permit
        var samplePermit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-2026-001",
            Title = "Hot Work Welding on CDU Main Line A1",
            Description = "Welding and pipe flange replacement on primary crude distillation unit discharge header.",
            ObjectiveDescription = "Replace corroded 8-inch carbon steel flange section and perform NDT weld testing.",
            PermitTypeId = hotWorkType.Id,
            Status = PermitStatus.Active,
            ZoneCode = zoneA.Code,
            ZoneId = zoneA.Id,
            IssuingAuthority = "Mohammed Zakee",
            SupervisorName = "Mohammed Zakee",
            SupervisorId = worker1.Id,
            ActivatedAt = DateTime.UtcNow,
            ScheduledStartTime = DateTime.UtcNow,
            ScheduledEndTime = DateTime.UtcNow.AddHours(8)
        };

        context.PermitRequests.Add(samplePermit);

        await context.SaveChangesAsync();
    }
}
