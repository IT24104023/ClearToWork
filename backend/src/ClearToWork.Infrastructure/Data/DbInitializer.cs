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
        // ─── 1. Seed Site & Zones ────────────────────────────────────────────
        Zone? zoneA = null;
        Zone? zoneB = null;

        if (!await context.Sites.AnyAsync())
        {
            var site = new Site
            {
                Id = Guid.NewGuid(),
                Name = "ClearToWork AI Industrial Refinery & Processing Plant",
                LocationCode = "PLANT-ALPHA-01",
                Latitude = 25.2048m,
                Longitude = 55.2708m
            };
            context.Sites.Add(site);

            zoneA = new Zone
            {
                Id = Guid.NewGuid(),
                SiteId = site.Id,
                Code = "ZONE-A1",
                Name = "Crude Distillation Unit Deck",
                Latitude = 25.2048,
                Longitude = 55.2708,
                RadiusMeters = 50,
                QrCodePayload = "QR-ZONE-A1-CRUDE-DECK",
                IsActive = true
            };

            zoneB = new Zone
            {
                Id = Guid.NewGuid(),
                SiteId = site.Id,
                Code = "ZONE-B2",
                Name = "Hydrocracker & Gas Storage Tank Farm",
                Latitude = 25.2055,
                Longitude = 55.2715,
                RadiusMeters = 75,
                QrCodePayload = "QR-ZONE-B2-HYDROCRACKER",
                IsActive = true
            };

            context.Zones.AddRange(zoneA, zoneB);
            await context.SaveChangesAsync();

            // Zone Adjacency (bidirectional)
            context.ZoneAdjacencies.AddRange(
                new ZoneAdjacency { ZoneId = zoneA.Id, AdjacentZoneId = zoneB.Id },
                new ZoneAdjacency { ZoneId = zoneB.Id, AdjacentZoneId = zoneA.Id }
            );
            await context.SaveChangesAsync();
        }
        else
        {
            zoneA = await context.Zones.FirstOrDefaultAsync(z => z.Code == "ZONE-A1")
                    ?? await context.Zones.FirstOrDefaultAsync();
            zoneB = await context.Zones.FirstOrDefaultAsync(z => z.Code == "ZONE-B2")
                    ?? await context.Zones.Skip(1).FirstOrDefaultAsync()
                    ?? zoneA;

            // Ensure adjacency exists if zones exist
            if (zoneA != null && zoneB != null && zoneA.Id != zoneB.Id && !await context.ZoneAdjacencies.AnyAsync())
            {
                context.ZoneAdjacencies.AddRange(
                    new ZoneAdjacency { ZoneId = zoneA.Id, AdjacentZoneId = zoneB.Id },
                    new ZoneAdjacency { ZoneId = zoneB.Id, AdjacentZoneId = zoneA.Id }
                );
                await context.SaveChangesAsync();
            }
        }

        // ─── 2. Seed Permit Types ────────────────────────────────────────────
        PermitType? hotWorkType = null;
        PermitType? coldWorkType = null;

        if (!await context.PermitTypes.AnyAsync())
        {
            hotWorkType = new PermitType
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

            coldWorkType = new PermitType
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
            await context.SaveChangesAsync();
        }
        else
        {
            hotWorkType = await context.PermitTypes.FirstOrDefaultAsync(p => p.Code == "HOT_WORK");
            coldWorkType = await context.PermitTypes.FirstOrDefaultAsync(p => p.Code == "COLD_WORK");
        }

        // ─── 3. Seed Users & Contractor ──────────────────────────────────────
        Worker? worker1 = null;

        if (!await context.Contractors.AnyAsync())
        {
            var contractor = new Contractor
            {
                Id = Guid.NewGuid(),
                CompanyName = "Global Energy Maintenance Corp",
                RegistrationNumber = "REG-2026-9901",
                SafetyRating = "A+"
            };
            context.Contractors.Add(contractor);

            worker1 = new Worker
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

            var worker3 = new Worker
            {
                Id = Guid.NewGuid(),
                BadgeNumber = "W-103",
                FirstName = "Dinithi",
                LastName = "Silva",
                Trade = "Site Compliance Supervisor",
                ContractorId = contractor.Id,
                IsActive = true
            };

            var worker4 = new Worker
            {
                Id = Guid.NewGuid(),
                BadgeNumber = "W-104",
                FirstName = "Oshini",
                LastName = "Dev",
                Trade = "System & Equipment Administrator",
                ContractorId = contractor.Id,
                IsActive = true
            };

            context.Workers.AddRange(worker1, worker2, worker3, worker4);
            await context.SaveChangesAsync();
        }
        else
        {
            worker1 = await context.Workers.FirstOrDefaultAsync();
        }

        // ─── 4. Seed Assets ──────────────────────────────────────────────────
        if (!await context.Assets.AnyAsync() && zoneA != null)
        {
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
            await context.SaveChangesAsync();

            // Seed Initial Calibration & Inspection Records
            var calGas = new CalibrationRecord
            {
                Id = Guid.NewGuid(),
                AssetId = gasMonitor.Id,
                CalibratedBy = "Dräger Safety Accredited Metrology Lab",
                CertificateNumber = "CAL-DRAGER-2026-9901",
                CalibrationDate = DateTime.UtcNow.AddDays(-10),
                NextCalibrationDate = DateTime.UtcNow.AddMonths(6),
                PassStatus = true
            };

            var inspGas = new InspectionRecord
            {
                Id = Guid.NewGuid(),
                AssetId = gasMonitor.Id,
                InspectorName = "Chemini Perera (HSE Gas Safety Specialist)",
                InspectionDate = DateTime.UtcNow.AddDays(-2),
                NextInspectionDate = DateTime.UtcNow.AddMonths(3),
                Passed = true,
                Notes = "LEL, O2, H2S, CO bump test verified OK. Sensor response within 10s. Clean flame arrestor."
            };

            var inspBreaker = new InspectionRecord
            {
                Id = Guid.NewGuid(),
                AssetId = isolationBreaker.Id,
                InspectorName = "Mohammed Zakee (Lead Isolation Tech)",
                InspectionDate = DateTime.UtcNow.AddDays(-5),
                NextInspectionDate = DateTime.UtcNow.AddMonths(6),
                Passed = true,
                Notes = "Visual check and mechanical interlock operational. Insulating barriers verified."
            };

            context.CalibrationRecords.Add(calGas);
            context.InspectionRecords.AddRange(inspGas, inspBreaker);
            await context.SaveChangesAsync();
        }
        else if (await context.Assets.AnyAsync())
        {
            var existingGas = await context.Assets.FirstOrDefaultAsync(a => a.AssetTag == "GAS-MON-401");
            if (existingGas != null && !await context.CalibrationRecords.AnyAsync(c => c.AssetId == existingGas.Id))
            {
                context.CalibrationRecords.Add(new CalibrationRecord
                {
                    Id = Guid.NewGuid(),
                    AssetId = existingGas.Id,
                    CalibratedBy = "Dräger Safety Accredited Metrology Lab",
                    CertificateNumber = "CAL-DRAGER-2026-9901",
                    CalibrationDate = DateTime.UtcNow.AddDays(-10),
                    NextCalibrationDate = DateTime.UtcNow.AddMonths(6),
                    PassStatus = true
                });
            }
            if (existingGas != null && !await context.InspectionRecords.AnyAsync(i => i.AssetId == existingGas.Id))
            {
                context.InspectionRecords.Add(new InspectionRecord
                {
                    Id = Guid.NewGuid(),
                    AssetId = existingGas.Id,
                    InspectorName = "Chemini Perera (HSE Gas Safety Specialist)",
                    InspectionDate = DateTime.UtcNow.AddDays(-2),
                    NextInspectionDate = DateTime.UtcNow.AddMonths(3),
                    Passed = true,
                    Notes = "LEL, O2, H2S, CO bump test verified OK. Sensor response within 10s."
                });
            }
            var existingBreaker = await context.Assets.FirstOrDefaultAsync(a => a.AssetTag == "SWGR-02-BKR-14");
            if (existingBreaker != null && !await context.InspectionRecords.AnyAsync(i => i.AssetId == existingBreaker.Id))
            {
                context.InspectionRecords.Add(new InspectionRecord
                {
                    Id = Guid.NewGuid(),
                    AssetId = existingBreaker.Id,
                    InspectorName = "Mohammed Zakee (Lead Isolation Tech)",
                    InspectionDate = DateTime.UtcNow.AddDays(-5),
                    NextInspectionDate = DateTime.UtcNow.AddMonths(6),
                    Passed = true,
                    Notes = "Visual check and mechanical interlock operational."
                });
            }
            await context.SaveChangesAsync();
        }

        // ─── 4b. Seed Initial Isolation Points (LOTO) ────────────────────────
        if (!await context.IsolationPoints.AnyAsync() && zoneA != null)
        {
            var isoPoint1 = new IsolationPoint
            {
                Id = Guid.NewGuid(),
                ZoneId = zoneA.Id,
                TagIdentifier = "ISO-CDU-VLV-01",
                Description = "Crude Distillation Primary Fuel Gas Shutoff Valve",
                Type = IsolationType.Mechanical,
                State = IsolationState.LockedOut,
                LockedAt = DateTime.UtcNow.AddDays(-1)
            };

            var isoPoint2 = new IsolationPoint
            {
                Id = Guid.NewGuid(),
                ZoneId = zoneA.Id,
                TagIdentifier = "ISO-SWGR-BKR-14",
                Description = "High Voltage Bus Feed Circuit Breaker Rack-Out",
                Type = IsolationType.Electrical,
                State = IsolationState.Open
            };

            context.IsolationPoints.AddRange(isoPoint1, isoPoint2);
            await context.SaveChangesAsync();
        }

        // ─── 5. Seed Initial Sample Permit ───────────────────────────────────
        if (!await context.PermitRequests.AnyAsync() && zoneA != null && hotWorkType != null)
        {
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
                SupervisorId = worker1?.Id ?? Guid.NewGuid(),
                ActivatedAt = DateTime.UtcNow,
                ScheduledStartTime = DateTime.UtcNow,
                ScheduledEndTime = DateTime.UtcNow.AddHours(8)
            };

            context.PermitRequests.Add(samplePermit);
            await context.SaveChangesAsync();
        }

        // ─── 6. Student 4: Seed Hazard Types, Controls & Incompatibility Rules ─
        if (!await context.HazardTypes.AnyAsync())
        {
            var hotWorkHazard = new HazardType
            {
                Id = Guid.NewGuid(),
                Code = "HOT_WORK",
                Name = "Hot Work & Open Flame Operations",
                Category = "Thermal / Ignition",
                SeverityLevel = HazardSeverity.High,
                MaxWindSpeedKmh = 35.0,
                ProhibitedInRain = false,
                ControlMeasures = new List<ControlMeasure>
                {
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-HW-01", RequirementDescription = "Continuous combustible gas testing (LEL < 1% before striking arc)", IsMandatory = true },
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-HW-02", RequirementDescription = "Certified fire watch stationed within 5m with 2x 9kg dry powder extinguishers", IsMandatory = true },
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-HW-03", RequirementDescription = "Fire-retardant spark containment habitat erected around joint", IsMandatory = true }
                }
            };

            var solventHazard = new HazardType
            {
                Id = Guid.NewGuid(),
                Code = "SOLVENT_PAINTING",
                Name = "Solvent Coating & Spray Painting",
                Category = "Flammable Atmosphere",
                SeverityLevel = HazardSeverity.High,
                MaxWindSpeedKmh = 25.0,
                ProhibitedInRain = true,
                ControlMeasures = new List<ControlMeasure>
                {
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-SP-01", RequirementDescription = "Forced explosion-proof ventilation maintaining solvent concentration below 10% LEL", IsMandatory = true },
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-SP-02", RequirementDescription = "Intrinsically safe lighting (ATEX Zone 1 certified) exclusively deployed", IsMandatory = true }
                }
            };

            var confinedHazard = new HazardType
            {
                Id = Guid.NewGuid(),
                Code = "CONFINED_SPACE",
                Name = "Confined Space Entry & Vessel Ingress",
                Category = "Atmospheric / Entrapment",
                SeverityLevel = HazardSeverity.Critical,
                MaxWindSpeedKmh = null,
                ProhibitedInRain = false,
                ControlMeasures = new List<ControlMeasure>
                {
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-CS-01", RequirementDescription = "Calibrated multi-gas detector testing O2, CO, H2S, and LEL at 3 elevation depths", IsMandatory = true },
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-CS-02", RequirementDescription = "Trained standby attendant stationed outside entrance with tripod winch extraction harness", IsMandatory = true }
                }
            };

            var heightHazard = new HazardType
            {
                Id = Guid.NewGuid(),
                Code = "WORKING_AT_HEIGHT",
                Name = "Working at Height (>2m Elevated Decks)",
                Category = "Physical / Fall Hazard",
                SeverityLevel = HazardSeverity.High,
                MaxWindSpeedKmh = 40.0,
                ProhibitedInRain = true,
                ControlMeasures = new List<ControlMeasure>
                {
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-WH-01", RequirementDescription = "Full body harness 100% tied off to certified anchor point rated for 15kN", IsMandatory = true },
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-WH-02", RequirementDescription = "Toe-boards and tool lanyards fastened to prevent dropped object hazards", IsMandatory = true }
                }
            };

            var electricalHazard = new HazardType
            {
                Id = Guid.NewGuid(),
                Code = "ELECTRICAL_ISOLATION",
                Name = "High Voltage Electrical Switching",
                Category = "Electrical Shock & Arc Flash",
                SeverityLevel = HazardSeverity.Critical,
                MaxWindSpeedKmh = null,
                ProhibitedInRain = true,
                ControlMeasures = new List<ControlMeasure>
                {
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-EL-01", RequirementDescription = "Live-Dead-Live electrical potential verification before earthing clamp attachment", IsMandatory = true },
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-EL-02", RequirementDescription = "Lockout-Tagout padlock and danger tag applied to main feed breaker handle", IsMandatory = true }
                }
            };

            var gasPurgeHazard = new HazardType
            {
                Id = Guid.NewGuid(),
                Code = "FLAMMABLE_GAS_TEST",
                Name = "Flammable Gas Purging & Hydrotest",
                Category = "Vapor Cloud & Overpressure",
                SeverityLevel = HazardSeverity.Critical,
                MaxWindSpeedKmh = 30.0,
                ProhibitedInRain = false,
                ControlMeasures = new List<ControlMeasure>
                {
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-FG-01", RequirementDescription = "Safety relief valve set to 110% maximum allowable working pressure (MAWP)", IsMandatory = true },
                    new ControlMeasure { Id = Guid.NewGuid(), Code = "CM-FG-02", RequirementDescription = "100m radial exclusion zone barricaded with reflective warning ribbons", IsMandatory = true }
                }
            };

            context.HazardTypes.AddRange(hotWorkHazard, solventHazard, confinedHazard, heightHazard, electricalHazard, gasPurgeHazard);
            await context.SaveChangesAsync();

            // Seed Incompatibility Rules linking these hazard types
            context.IncompatibilityRules.AddRange(
                new IncompatibilityRule
                {
                    Id = Guid.NewGuid(),
                    RuleCode = "HR-07",
                    PrimaryHazardId = hotWorkHazard.Id,
                    ConflictingHazardId = solventHazard.Id,
                    Reason = "Open sparks and welding arcs can ignite flammable solvent vapors, creating an explosive flash-fire condition.",
                    AppliesToAdjacentZones = true
                },
                new IncompatibilityRule
                {
                    Id = Guid.NewGuid(),
                    RuleCode = "HR-08",
                    PrimaryHazardId = hotWorkHazard.Id,
                    ConflictingHazardId = gasPurgeHazard.Id,
                    Reason = "Naked flame operations adjacent to pressurized hydrocarbon gas venting or purging introduce critical catastrophic ignition risk.",
                    AppliesToAdjacentZones = true
                },
                new IncompatibilityRule
                {
                    Id = Guid.NewGuid(),
                    RuleCode = "HR-09",
                    PrimaryHazardId = confinedHazard.Id,
                    ConflictingHazardId = solventHazard.Id,
                    Reason = "Heavier-than-air solvent vapor clouds accumulate in low points and confined space ingress paths, causing severe asphyxiation and acute toxicity.",
                    AppliesToAdjacentZones = true
                },
                new IncompatibilityRule
                {
                    Id = Guid.NewGuid(),
                    RuleCode = "HR-10",
                    PrimaryHazardId = heightHazard.Id,
                    ConflictingHazardId = hotWorkHazard.Id,
                    Reason = "Dropping slag or hot sparks directly onto workers, fall arrest harnesses, or safety lanyards below compromises synthetic fiber structural integrity.",
                    AppliesToAdjacentZones = false
                }
            );
            await context.SaveChangesAsync();
        }
        else if (!await context.IncompatibilityRules.AnyAsync())
        {
            // In case HazardTypes were present but IncompatibilityRules was empty
            var hotWork = await context.HazardTypes.FirstOrDefaultAsync(h => h.Code == "HOT_WORK");
            var solvent = await context.HazardTypes.FirstOrDefaultAsync(h => h.Code == "SOLVENT_PAINTING");
            var confined = await context.HazardTypes.FirstOrDefaultAsync(h => h.Code == "CONFINED_SPACE");

            if (hotWork != null && solvent != null)
            {
                context.IncompatibilityRules.Add(new IncompatibilityRule
                {
                    Id = Guid.NewGuid(),
                    RuleCode = "HR-07",
                    PrimaryHazardId = hotWork.Id,
                    ConflictingHazardId = solvent.Id,
                    Reason = "Open sparks and welding arcs can ignite flammable solvent vapors, creating an explosive flash-fire condition.",
                    AppliesToAdjacentZones = true
                });
            }

            if (confined != null && solvent != null)
            {
                context.IncompatibilityRules.Add(new IncompatibilityRule
                {
                    Id = Guid.NewGuid(),
                    RuleCode = "HR-09",
                    PrimaryHazardId = confined.Id,
                    ConflictingHazardId = solvent.Id,
                    Reason = "Heavier-than-air solvent vapor clouds accumulate in low points and confined space ingress paths, causing severe asphyxiation and acute toxicity.",
                    AppliesToAdjacentZones = true
                });
            }

            await context.SaveChangesAsync();
        }

        // ─── 7. Student 4: Seed Initial Safety Observations ──────────────────
        if (!await context.Observations.AnyAsync() && zoneA != null && zoneB != null)
        {
            context.Observations.AddRange(
                new Observation
                {
                    Id = Guid.NewGuid(),
                    ZoneId = zoneA.Id,
                    Title = "Atmospheric Monitor Calibration Tag Expired on CDU Level 2",
                    Description = "Dräger multi-gas detection unit displayed calibration sticker dated over 90 days ago. Immediate bump-test and recalibration requested.",
                    Severity = "Medium",
                    Category = "Equipment & Monitoring",
                    ReportedBy = "Chemini Perera",
                    ReportedByUserId = worker1?.Id ?? Guid.NewGuid(),
                    ObservedAt = DateTime.UtcNow.AddHours(-14),
                    LoggedAt = DateTime.UtcNow.AddHours(-14)
                },
                new Observation
                {
                    Id = Guid.NewGuid(),
                    ZoneId = zoneB.Id,
                    Title = "Solvent Paint Drums Stored in Non-Bunded Secondary Containment",
                    Description = "Three 20-liter thinner drums identified outside designated chemical storage locker, within 15 meters of transfer line flange.",
                    Severity = "High",
                    Category = "Housekeeping & Storage",
                    ReportedBy = "Chemini Perera",
                    ReportedByUserId = worker1?.Id ?? Guid.NewGuid(),
                    ObservedAt = DateTime.UtcNow.AddHours(-5),
                    LoggedAt = DateTime.UtcNow.AddHours(-5)
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
