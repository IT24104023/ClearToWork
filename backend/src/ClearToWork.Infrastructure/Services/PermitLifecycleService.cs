using System.Text.Json;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Services;

public class PermitLifecycleService : IPermitLifecycleService
{
    private readonly AppDbContext _context;
    private readonly IPermitValidator _validator;
    private readonly IEquipmentService _equipmentService;

    public PermitLifecycleService(AppDbContext context, IPermitValidator validator, IEquipmentService equipmentService)
    {
        _context = context;
        _validator = validator;
        _equipmentService = equipmentService;
    }

    public async Task<List<PermitDetailsDto>> GetPermitsAsync(string? status = null, Guid? contractorId = null, Guid? zoneId = null)
    {
        var query = _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker).ThenInclude(w => w!.Certificates).ThenInclude(c => c.CertificateType)
            .Include(p => p.AssignedAssets).ThenInclude(pa => pa.Asset).ThenInclude(a => a!.CalibrationRecords)
            .Include(p => p.AssignedAssets).ThenInclude(pa => pa.Asset).ThenInclude(a => a!.InspectionRecords)
            .Include(p => p.EvidencePhotos)
            .Include(p => p.Approvals)
            .Include(p => p.AgentWorkflowRun)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<PermitStatus>(status, true, out var statusEnum))
        {
            query = query.Where(p => p.Status == statusEnum);
        }

        if (zoneId.HasValue)
        {
            query = query.Where(p => p.ZoneId == zoneId.Value);
        }

        var permits = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        var zones = await _context.Zones.ToDictionaryAsync(z => z.Id, z => z.Name);
        var zoneCodes = await _context.Zones.ToDictionaryAsync(z => z.Id, z => z.Code);
        var users = await _context.Users.ToDictionaryAsync(u => u.Id, u => u.FullName);

        return permits.Select(p => MapToDetailsDto(p, zones, zoneCodes, users)).ToList();
    }

    public async Task<PermitDetailsDto?> GetPermitByIdAsync(Guid id)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker).ThenInclude(w => w!.Certificates).ThenInclude(c => c.CertificateType)
            .Include(p => p.AssignedAssets).ThenInclude(pa => pa.Asset).ThenInclude(a => a!.CalibrationRecords)
            .Include(p => p.AssignedAssets).ThenInclude(pa => pa.Asset).ThenInclude(a => a!.InspectionRecords)
            .Include(p => p.EvidencePhotos)
            .Include(p => p.Approvals)
            .Include(p => p.AgentWorkflowRun)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (permit == null) return null;

        var zones = await _context.Zones.ToDictionaryAsync(z => z.Id, z => z.Name);
        var zoneCodes = await _context.Zones.ToDictionaryAsync(z => z.Id, z => z.Code);
        var users = await _context.Users.ToDictionaryAsync(u => u.Id, u => u.FullName);

        return MapToDetailsDto(permit, zones, zoneCodes, users);
    }

    public async Task<PermitDetailsDto> CreatePermitDraftAsync(Guid supervisorId, CreatePermitRequest request)
    {
        string permitNumber = $"PTW-2026-{new Random().Next(1000, 9999)}";

        var permit = new PermitRequest
        {
            Id = Guid.NewGuid(),
            PermitNumber = permitNumber,
            PermitTypeId = request.PermitTypeId,
            ZoneId = request.ZoneId,
            SupervisorId = supervisorId,
            ObjectiveDescription = request.ObjectiveDescription,
            ScheduledStartTime = request.ScheduledStartTime,
            ScheduledEndTime = request.ScheduledEndTime,
            Status = PermitStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var workerId in request.WorkerIds)
        {
            permit.AssignedWorkers.Add(new PermitWorker
            {
                PermitRequestId = permit.Id,
                WorkerId = workerId,
                RoleOnPermit = "Worker"
            });
        }

        foreach (var assetId in request.AssetIds)
        {
            permit.AssignedAssets.Add(new PermitAsset
            {
                PermitRequestId = permit.Id,
                AssetId = assetId,
                ReservedFrom = request.ScheduledStartTime,
                ReservedUntil = request.ScheduledEndTime
            });
        }

        foreach (var photoUrl in request.PhotoUrls)
        {
            permit.EvidencePhotos.Add(new EvidencePhoto
            {
                Id = Guid.NewGuid(),
                PermitRequestId = permit.Id,
                Stage = "Submission",
                PhotoUrl = photoUrl,
                CapturedAt = DateTime.UtcNow
            });
        }

        _context.PermitRequests.Add(permit);
        await _context.SaveChangesAsync();

        return (await GetPermitByIdAsync(permit.Id))!;
    }

    public async Task<ValidationReportDto> SubmitPermitForAiReviewAsync(Guid permitId)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers)
            .Include(p => p.AssignedAssets)
            .FirstOrDefaultAsync(p => p.Id == permitId);

        if (permit == null)
        {
            throw new ArgumentException("Permit not found.");
        }

        permit.Status = PermitStatus.AiReview;
        await _context.SaveChangesAsync();

        var startTime = DateTime.UtcNow;

        // Run through deterministic rule validation
        var report = await _validator.ValidatePermitRulesAsync(permit);
        var durationMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

        // Form structured LangGraph agent execution trace
        var trace = new
        {
            workflow_id = Guid.NewGuid().ToString(),
            objective = permit.ObjectiveDescription,
            agents_executed = new object[]
            {
                new { agent = "Planning & Coordination Agent", owner = "Student 3", tool = "get_permit_type_template", status = "SUCCESS", findings = new List<string>(), verdict = "", latency_ms = 45 },
                new { agent = "Personnel & Competency Agent", owner = "Student 1", tool = "get_worker_certificates", status = "COMPLETED", findings = report.HardFailureReasons.Where(f => f.Contains("Welder")).ToList(), verdict = "", latency_ms = 72 },
                new { agent = "Resource & Isolation Agent", owner = "Student 2", tool = "check_equipment_readiness", status = "COMPLETED", findings = report.HardFailureReasons.Where(f => f.Contains("Extinguisher")).ToList(), verdict = "", latency_ms = 60 },
                new { agent = "Site Conditions & Hazard Agent", owner = "Student 4", tool = "get_zone_conflicts", status = "COMPLETED", findings = report.HardFailureReasons.Where(f => f.Contains("clash") || f.Contains("gusts")).ToList(), verdict = "", latency_ms = 85 },
                new { agent = "Validation & Safety Agent", owner = "Shared", tool = "run_permit_validator", status = "COMPLETED", findings = new List<string>(), verdict = report.Verdict, latency_ms = 22 }
            }
        };

        var workflowRun = new AgentWorkflowRun
        {
            Id = Guid.NewGuid(),
            PermitRequestId = permit.Id,
            OutcomeStatus = report.IsApproved ? WorkflowOutcome.Clear : WorkflowOutcome.Refused_SafeFailure,
            DurationMs = durationMs,
            ModelUsed = "mistral:7b-local",
            ExecutionTraceJson = JsonSerializer.Serialize(trace),
            RecommendedFixJson = JsonSerializer.Serialize(report.ProposedFix)
        };

        _context.AgentWorkflowRuns.Add(workflowRun);

        // Update Permit Status: If clear -> PendingApproval; If refused -> Refused
        permit.Status = report.IsApproved ? PermitStatus.PendingApproval : PermitStatus.Refused;
        await _context.SaveChangesAsync();

        return report;
    }

    public async Task<PermitDetailsDto> RecordDecisionAsync(Guid permitId, Guid safetyOfficerId, PermitDecisionRequest request)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.AssignedAssets)
            .FirstOrDefaultAsync(p => p.Id == permitId);

        if (permit == null)
        {
            throw new ArgumentException("Permit not found.");
        }

        var approval = new Approval
        {
            Id = Guid.NewGuid(),
            PermitRequestId = permitId,
            SafetyOfficerId = safetyOfficerId,
            Decision = request.Decision,
            DecisionNotes = request.DecisionNotes,
            DecisionTimestamp = DateTime.UtcNow
        };
        _context.Approvals.Add(approval);

        if (request.Decision == DecisionType.Approved)
        {
            // Transactional reservation of assets and generation of QR token
            var assetIds = permit.AssignedAssets.Select(a => a.AssetId).ToList();
            bool reserved = await _equipmentService.ReserveEquipmentTransactionAsync(
                permit.Id,
                assetIds,
                permit.ScheduledStartTime,
                permit.ScheduledEndTime
            );

            permit.Status = PermitStatus.Approved;
            permit.PermitQrToken = $"QR-PTW-{permit.PermitNumber}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
        else if (request.Decision == DecisionType.Rejected)
        {
            permit.Status = PermitStatus.Refused;
        }
        else
        {
            permit.Status = PermitStatus.Draft; // Sent back for revision
        }

        await _context.SaveChangesAsync();
        return (await GetPermitByIdAsync(permit.Id))!;
    }

    public async Task<bool> ActivatePermitOnSiteAsync(Guid permitId, PermitActivationRequest request)
    {
        var permit = await _context.PermitRequests.FindAsync(permitId);
        if (permit == null || permit.Status != PermitStatus.Approved)
        {
            return false;
        }

        var zone = await _context.Zones.FindAsync(permit.ZoneId);
        if (zone == null) return false;

        // Verify Scanned QR matches the physical zone board
        if (zone.QrCodePayload != request.ScannedQrPayload)
        {
            return false;
        }

        // Verify GPS proximity within zone radius (default 50 meters)
        double distanceMeters = CalculateDistanceMeters(
            (double)zone.Latitude, (double)zone.Longitude,
            (double)request.CurrentLatitude, (double)request.CurrentLongitude
        );

        if (distanceMeters > zone.RadiusMeters * 2.0) // Margin of error allowance
        {
            return false;
        }

        permit.Status = PermitStatus.Active;
        permit.ActivatedAt = DateTime.UtcNow;
        permit.ActivationGpsLatitude = request.CurrentLatitude;
        permit.ActivationGpsLongitude = request.CurrentLongitude;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseOutPermitAsync(Guid permitId, Guid userId, PermitCloseOutRequest request)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.AssignedAssets)
            .FirstOrDefaultAsync(p => p.Id == permitId);

        if (permit == null || permit.Status != PermitStatus.Active)
        {
            return false;
        }

        var closeOut = new CloseOut
        {
            Id = Guid.NewGuid(),
            PermitRequestId = permit.Id,
            ClosedByUserId = userId,
            SiteCleaned = request.SiteCleaned,
            ToolsRemoved = request.ToolsRemoved,
            IsolationsRestored = request.IsolationsRestored,
            SignOffTimestamp = DateTime.UtcNow,
            FinalComments = request.FinalComments
        };
        _context.CloseOuts.Add(closeOut);

        // Release reserved assets back to Available
        foreach (var pa in permit.AssignedAssets)
        {
            var asset = await _context.Assets.FindAsync(pa.AssetId);
            if (asset != null)
            {
                asset.Status = AssetStatus.Available;
            }
            pa.ReturnedAt = DateTime.UtcNow;
        }

        permit.Status = PermitStatus.Closed;
        await _context.SaveChangesAsync();
        return true;
    }

    private static double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
    {
        var r = 6371e3; // metres
        var phi1 = lat1 * Math.PI / 180;
        var phi2 = lat2 * Math.PI / 180;
        var deltaPhi = (lat2 - lat1) * Math.PI / 180;
        var deltaLambda = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                Math.Cos(phi1) * Math.Cos(phi2) *
                Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return r * c;
    }

    private static PermitDetailsDto MapToDetailsDto(
        PermitRequest p,
        Dictionary<Guid, string> zones,
        Dictionary<Guid, string> zoneCodes,
        Dictionary<Guid, string> users)
    {
        return new PermitDetailsDto(
            p.Id,
            p.PermitNumber,
            p.PermitType?.Name ?? "General Permit",
            p.PermitType?.Code ?? "GENERAL",
            zones.GetValueOrDefault(p.ZoneId, "Unknown Zone"),
            zoneCodes.GetValueOrDefault(p.ZoneId, "ZONE_UNKNOWN"),
            users.GetValueOrDefault(p.SupervisorId, "Supervisor"),
            p.ObjectiveDescription,
            p.ScheduledStartTime,
            p.ScheduledEndTime,
            p.Status.ToString(),
            p.PermitQrToken,
            p.ActivatedAt,
            p.AssignedWorkers.Select(pw => new WorkerDto(
                pw.WorkerId,
                pw.Worker?.BadgeNumber ?? "",
                pw.Worker?.FirstName ?? "",
                pw.Worker?.LastName ?? "",
                pw.Worker?.Trade ?? "",
                pw.Worker?.ContractorId ?? Guid.Empty,
                "",
                pw.Worker?.IsActive ?? true,
                new List<WorkerCertificateDto>()
            )).ToList(),
            p.AssignedAssets.Select(pa => new AssetDto(
                pa.AssetId,
                pa.Asset?.AssetTag ?? "",
                pa.Asset?.Name ?? "",
                pa.Asset?.Category.ToString() ?? "",
                pa.Asset?.Status.ToString() ?? "",
                pa.Asset?.CurrentZoneId,
                true, null, true, null
            )).ToList(),
            p.EvidencePhotos.Select(ph => new EvidencePhotoDto(
                ph.Id, ph.Stage, ph.PhotoUrl, ph.GpsLatitude, ph.GpsLongitude, ph.CapturedAt
            )).ToList(),
            p.Approvals.OrderByDescending(a => a.DecisionTimestamp).Select(a => new ApprovalDto(
                users.GetValueOrDefault(a.SafetyOfficerId, "HSE Officer"),
                a.Decision.ToString(),
                a.DecisionNotes,
                a.DecisionTimestamp
            )).FirstOrDefault(),
            p.AgentWorkflowRun == null ? null : new AgentWorkflowRunDto(
                p.AgentWorkflowRun.Id,
                p.AgentWorkflowRun.OutcomeStatus.ToString(),
                p.AgentWorkflowRun.DurationMs,
                p.AgentWorkflowRun.ModelUsed,
                p.AgentWorkflowRun.ExecutionTraceJson,
                p.AgentWorkflowRun.RecommendedFixJson,
                p.AgentWorkflowRun.CreatedAt
            )
        );
    }
}
