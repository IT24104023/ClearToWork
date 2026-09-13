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
        if (await context.Users.AnyAsync())
        {
            return; // Already seeded
        }

        // 1. Seed Site & Zones
        var site = new Site
        {
            Id = Guid.NewGuid(),
            Name = "Apex Industrial Petrochemical Plant",
            Code = "SITE-APEX-01",
            Latitude = 6.9271m,
            Longitude = 79.8612m
        };
        context.Sites.Add(site);

        var zoneB3 = new Zone
        {
            Id = Guid.NewGuid(),
            SiteId = site.Id,
            Code = "ZONE_B3",
            Name = "Mezzanine Floor",
            Latitude = 6.9271m,
            Longitude = 79.8612m,
            RadiusMeters = 50.0,
            QrCodePayload = "QR-ZONE-B3-MEZZANINE",
            IsActive = true
        };

        var zoneB4 = new Zone
        {
            Id = Guid.NewGuid(),
            SiteId = site.Id,
            Code = "ZONE_B4",
            Name = "Paint & Solvent Storage",
            Latitude = 6.9272m,
            Longitude = 79.8614m,
            RadiusMeters = 40.0,
            QrCodePayload = "QR-ZONE-B4-PAINTSTORE",
            IsActive = true
        };
        context.Zones.AddRange(zoneB3, zoneB4);

        // Zone Adjacency (B3 <-> B4)
        context.ZoneAdjacencies.AddRange(
            new ZoneAdjacency { ZoneId = zoneB3.Id, AdjacentZoneId = zoneB4.Id, RiskTransferLevel = "High" },
            new ZoneAdjacency { ZoneId = zoneB4.Id, AdjacentZoneId = zoneB3.Id, RiskTransferLevel = "High" }
        );

        // 2. Seed Hazard Types & Rules
        var hotWorkHazard = new HazardType
        {
            Id = Guid.NewGuid(),
            Code = "HOT_WORK",
            Name = "Hot Work (Welding, Cutting, Grinding)",
            SeverityLevel = HazardSeverity.High,
            MaxWindSpeedKmh = 35.0,
            ProhibitedInRain = true
        };

        var solventHazard = new HazardType
        {
            Id = Guid.NewGuid(),
            Code = "SOLVENT_VAPOUR",
            Name = "Volatile Solvent & Spray Painting",
            SeverityLevel = HazardSeverity.High,
            MaxWindSpeedKmh = 45.0,
            ProhibitedInRain = false
        };
        context.HazardTypes.AddRange(hotWorkHazard, solventHazard);

        // Rule HR-07: Incompatibility
        context.IncompatibilityRules.Add(new IncompatibilityRule
        {
            Id = Guid.NewGuid(),
            RuleCode = "HR-07",
            PrimaryHazardId = hotWorkHazard.Id,
            ConflictingHazardId = solventHazard.Id,
            Reason = "Hot work beside solvent vapour is forbidden due to atmospheric explosive risk.",
            AppliesToAdjacentZones = true
        });

        // 3. Seed Permit Types
        var hotWorkPermitType = new PermitType
        {
            Id = Guid.NewGuid(),
            Code = "HOT_WORK",
            Name = "Hot Work Operational Permit",
            Description = "Required for open flame, spark-producing tools, welding, or cutting.",
            MaxDurationHours = 8,
            RequiresFireWatch = true
        };

        var paintingPermitType = new PermitType
        {
            Id = Guid.NewGuid(),
            Code = "SOLVENT_PAINTING",
            Name = "Solvent Application Permit",
            Description = "Required for industrial painting or solvent coating operations.",
            MaxDurationHours = 6,
            RequiresFireWatch = false
        };
        context.PermitTypes.AddRange(hotWorkPermitType, paintingPermitType);

        // 4. Seed Contractor & Workers
        var contractor = new Contractor
        {
            Id = Guid.NewGuid(),
            CompanyName = "Apex Industrial Engineering Ltd",
            LicenseNumber = "LIC-IND-2024-884",
            ContactEmail = "operations@apexengineering.com"
        };
        context.Contractors.Add(contractor);

        var hotWorkCertType = new CertificateType
        {
            Id = Guid.NewGuid(),
            Code = "CERT-HW-01",
            Name = "Certified Industrial Hot-Work Welder",
            RequiredForTrade = "Welder",
            ValidityMonths = 24
        };
        context.CertificateTypes.Add(hotWorkCertType);

        // Welder W-1182: Expired 3 days ago (Demo Scenario)
        var welderExpired = new Worker
        {
            Id = Guid.NewGuid(),
            BadgeNumber = "W-1182",
            FirstName = "Michael",
            LastName = "Vance",
            Trade = "Welder",
            ContractorId = contractor.Id,
            IsActive = true
        };

        // Welder W-1204: Valid until 2027 (Recommended Replacement)
        var welderValid = new Worker
        {
            Id = Guid.NewGuid(),
            BadgeNumber = "W-1204",
            FirstName = "Sarah",
            LastName = "Connor",
            Trade = "Welder",
            ContractorId = contractor.Id,
            IsActive = true
        };
        context.Workers.AddRange(welderExpired, welderValid);

        context.WorkerCertificates.AddRange(
            new WorkerCertificate
            {
                Id = Guid.NewGuid(),
                WorkerId = welderExpired.Id,
                CertificateTypeId = hotWorkCertType.Id,
                CertificateNumber = "CERT-2024-1182",
                IssuingBody = "National Welding Board",
                IssueDate = DateTime.UtcNow.AddMonths(-24).AddDays(-3),
                ExpiryDate = DateTime.UtcNow.AddDays(-3), // Expired 3 days ago
                Status = CertificateStatus.Expired
            },
            new WorkerCertificate
            {
                Id = Guid.NewGuid(),
                WorkerId = welderValid.Id,
                CertificateTypeId = hotWorkCertType.Id,
                CertificateNumber = "CERT-2025-1204",
                IssuingBody = "National Welding Board",
                IssueDate = DateTime.UtcNow.AddMonths(-6),
                ExpiryDate = DateTime.UtcNow.AddMonths(18), // Valid to mid-2027
                Status = CertificateStatus.Valid
            }
        );

        // 5. Seed Equipment & Assets
        var extinguisherOverdue = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = "EX-22",
            Name = "CO2 Fire Extinguisher 5kg",
            Category = AssetCategory.Extinguisher,
            Status = AssetStatus.Available,
            CurrentZoneId = zoneB3.Id
        };

        var extinguisherValid = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = "EX-31",
            Name = "Dry Powder Extinguisher 9kg",
            Category = AssetCategory.Extinguisher,
            Status = AssetStatus.Available,
            CurrentZoneId = zoneB3.Id
        };

        var gasDetectorValid = new Asset
        {
            Id = Guid.NewGuid(),
            AssetTag = "GAS-DET-01",
            Name = "Multi-Gas 4-Sensor Monitor",
            Category = AssetCategory.GasDetector,
            Status = AssetStatus.Available,
            CurrentZoneId = zoneB3.Id
        };
        context.Assets.AddRange(extinguisherOverdue, extinguisherValid, gasDetectorValid);

        // Inspection record: EX-22 is 9 days overdue
        context.InspectionRecords.AddRange(
            new InspectionRecord
            {
                Id = Guid.NewGuid(),
                AssetId = extinguisherOverdue.Id,
                InspectionDate = DateTime.UtcNow.AddDays(-39),
                NextInspectionDate = DateTime.UtcNow.AddDays(-9), // 9 days overdue
                InspectorName = "Safety Team A",
                Passed = false,
                Notes = "Monthly inspection overdue."
            },
            new InspectionRecord
            {
                Id = Guid.NewGuid(),
                AssetId = extinguisherValid.Id,
                InspectionDate = DateTime.UtcNow.AddDays(-5),
                NextInspectionDate = DateTime.UtcNow.AddDays(25), // In date
                InspectorName = "Safety Team A",
                Passed = true,
                Notes = "Pressure gauge green, seal intact."
            }
        );

        context.CalibrationRecords.Add(new CalibrationRecord
        {
            Id = Guid.NewGuid(),
            AssetId = gasDetectorValid.Id,
            CalibrationDate = DateTime.UtcNow.AddDays(-10),
            NextCalibrationDate = DateTime.UtcNow.AddDays(80),
            CalibratedBy = "CalibTech Labs",
            CertificateNumber = "CAL-2026-993",
            PassStatus = true
        });

        // 6. Seed Users
        var supervisor = new User
        {
            Id = Guid.NewGuid(),
            FullName = "David Miller (Contractor Supervisor)",
            Email = "supervisor@contractor.com",
            // BCrypt hash of "Password123!" or simple secure hash
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.ContractorSupervisor,
            ContractorId = contractor.Id,
            IsActive = true
        };

        var safetyOfficer = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Elena Rostova (HSE Safety Officer)",
            Email = "safety@cleartowork.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.SafetyOfficer,
            IsActive = true
        };

        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "System Administrator",
            Email = "admin@cleartowork.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.Administrator,
            IsActive = true
        };
        context.Users.AddRange(supervisor, safetyOfficer, admin);

        // 7. Seed Active Conflicting Permit in adjacent Zone B4 (PTW-2026-0403)
        var conflictingPermit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = "PTW-2026-0403",
            PermitTypeId = paintingPermitType.Id,
            ZoneId = zoneB4.Id,
            SupervisorId = supervisor.Id,
            ObjectiveDescription = "Industrial solvent coating and spray painting of structural steelwork.",
            ScheduledStartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9),  // Tomorrow 09:00
            ScheduledEndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(12),    // Tomorrow 12:00
            Status = PermitStatus.Approved,
            PermitQrToken = "QR-PTW-2026-0403-ACTIVE"
        };
        context.PermitRequests.Add(conflictingPermit);

        await context.SaveChangesAsync();
    }
}
