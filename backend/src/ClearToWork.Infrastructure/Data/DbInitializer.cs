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
            RequiresFireWatch = true,
            RequiresGasTesting = true,
            MandatoryControlsJson = """["Dry powder extinguisher (9kg) within 5m","Continuous LEL/O2 gas monitoring","Fire-resistant welding blanket erected","Fire watch posted for 30 minutes post-completion","Hot work area barriered with hazard tape"]"""
        };

        var paintingPermitType = new PermitType
        {
            Id = Guid.NewGuid(),
            Code = "SOLVENT_PAINTING",
            Name = "Solvent Application Permit",
            Description = "Required for industrial painting or solvent coating operations.",
            MaxDurationHours = 6,
            RequiresFireWatch = false,
            RequiresGasTesting = true,
            MandatoryControlsJson = """["LEL gas monitoring active throughout","No ignition sources within 10m","Adequate forced ventilation confirmed","PPE: respirator (OV/P100), chemical-resistant gloves, face shield","Spill containment trays under all containers"]"""
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.ContractorSupervisor,
            ContractorId = contractor.Id,
            IsActive = true,
            AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&auto=format&fit=crop&q=80",
            PhoneNumber = "+94 77 123 4567",
            Department = "Apex Structural Fabrication & Welding",
            Bio = "Lead Contractor Field Supervisor with 12+ years experience in high-risk petrochem facilities."
        };

        var safetyOfficer = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Elena Rostova (HSE Safety Officer)",
            Email = "safety@cleartowork.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.SafetyOfficer,
            IsActive = true,
            AvatarUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=150&auto=format&fit=crop&q=80",
            PhoneNumber = "+94 71 987 6543",
            Department = "Corporate Health, Safety & Environment (HSE)",
            Bio = "Senior HSE Compliance Officer managing SIMOPS permits and hazardous atmosphere safety envelopes."
        };

        var admin = new User
        {
            Id = Guid.NewGuid(),
            FullName = "System Administrator",
            Email = "admin@cleartowork.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.Administrator,
            IsActive = true,
            AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&auto=format&fit=crop&q=80",
            PhoneNumber = "+94 11 234 5678",
            Department = "Operations & AI Systems Engineering",
            Bio = "ClearToWork AI Platform Administrator and LangGraph Orchestration Overseer."
        };
        context.Users.AddRange(supervisor, safetyOfficer, admin);

        // AreaSupervisor: Plant Area Supervisor (4th role per RBAC documentation)
        var areaSupervisor = new User
        {
            Id = Guid.NewGuid(),
            FullName = "James Whitfield (Area Supervisor)",
            Email = "areasup@cleartowork.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = UserRole.AreaSupervisor,
            IsActive = true,
            AvatarUrl = "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=150&auto=format&fit=crop&q=80",
            PhoneNumber = "+94 77 456 7890",
            Department = "Plant Operations — Mezzanine Block B",
            Bio = "Area Supervisor responsible for Zone B3 and B4 spatial isolation, SIMOPS monitoring, and site access control."
        };
        context.Users.Add(areaSupervisor);

        // 8. Seed Isolation Points for Zone B3 (referenced by agent get_isolation_points tool & Equipment page)
        context.IsolationPoints.AddRange(
            new IsolationPoint
            {
                Id = Guid.NewGuid(),
                ZoneId = zoneB3.Id,
                TagIdentifier = "ISO-B3-VALVE-01",
                Description = "Solvent supply isolation manifold — Lock-Out / Tag-Out point",
                Type = IsolationType.Valve,
                State = IsolationState.LockedOut,
                LockedByUserId = areaSupervisor.Id.ToString(),
                LockedAt = DateTime.UtcNow.AddHours(-2)
            },
            new IsolationPoint
            {
                Id = Guid.NewGuid(),
                ZoneId = zoneB3.Id,
                TagIdentifier = "ISO-B3-ELEC-04",
                Description = "415V Main busbar isolator switch — Tag-Out point",
                Type = IsolationType.Electrical,
                State = IsolationState.TaggedOut,
                LockedByUserId = areaSupervisor.Id.ToString(),
                LockedAt = DateTime.UtcNow.AddHours(-1)
            }
        );

        await context.SaveChangesAsync();
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
