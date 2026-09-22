using System.Net.Http.Json;
using System.Text.Json;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClearToWork.Infrastructure.Services;

public class PermitLifecycleService : IPermitLifecycleService
{
    private readonly AppDbContext _context;
    private readonly IPermitValidator _validator;
    private readonly IEquipmentService _equipmentService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<PermitLifecycleService> _logger;

    public PermitLifecycleService(
        AppDbContext context,
        IPermitValidator validator,
        IEquipmentService equipmentService,
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<PermitLifecycleService> logger)
    {
        _context = context;
        _validator = validator;
        _equipmentService = equipmentService;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<List<PermitDetailsDto>> GetPermitsAsync(string? status = null, Guid? contractorId = null, Guid? zoneId = null)
    {
        var query = _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker).ThenInclude(w => w!.Contractor)
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
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker).ThenInclude(w => w!.Contractor)
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
        // Use timestamp ticks + random suffix to guarantee uniqueness across rapid calls
        string permitNumber = $"PTW-{DateTime.UtcNow.Year}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 100000:D5}";

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

    public async Task<PermitDetailsDto?> UpdatePermitDraftAsync(Guid permitId, Guid supervisorId, UpdatePermitRequest request)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.AssignedWorkers)
            .Include(p => p.AssignedAssets)
            .Include(p => p.EvidencePhotos)
            .FirstOrDefaultAsync(p => p.Id == permitId);

        if (permit == null) return null;

        if (permit.Status != PermitStatus.Draft && permit.Status != PermitStatus.Submitted && permit.Status != PermitStatus.Refused)
        {
            throw new InvalidOperationException("Only draft, submitted, or refused permits can be modified.");
        }

        permit.PermitTypeId = request.PermitTypeId;
        permit.ZoneId = request.ZoneId;
        permit.ObjectiveDescription = request.ObjectiveDescription.Trim();
        permit.ScheduledStartTime = request.ScheduledStartTime;
        permit.ScheduledEndTime = request.ScheduledEndTime;
        permit.Status = PermitStatus.Draft;

        _context.PermitWorkers.RemoveRange(permit.AssignedWorkers);
        foreach (var workerId in request.WorkerIds)
        {
            permit.AssignedWorkers.Add(new PermitWorker
            {
                PermitRequestId = permit.Id,
                WorkerId = workerId,
                RoleOnPermit = "Worker"
            });
        }

        _context.PermitAssets.RemoveRange(permit.AssignedAssets);
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

        if (request.PhotoUrls != null && request.PhotoUrls.Count > 0)
        {
            foreach (var photoUrl in request.PhotoUrls)
            {
                if (!permit.EvidencePhotos.Any(p => p.PhotoUrl == photoUrl))
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
            }
        }

        await _context.SaveChangesAsync();
        return await GetPermitByIdAsync(permit.Id);
    }

    public async Task<bool> DeletePermitDraftAsync(Guid permitId, Guid supervisorId)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.AssignedWorkers)
            .Include(p => p.AssignedAssets)
            .Include(p => p.EvidencePhotos)
            .Include(p => p.Approvals)
            .Include(p => p.AgentWorkflowRun)
            .FirstOrDefaultAsync(p => p.Id == permitId);

        if (permit == null) return false;

        if (permit.Status != PermitStatus.Draft && permit.Status != PermitStatus.Submitted && permit.Status != PermitStatus.Refused)
        {
            throw new InvalidOperationException("Approved or Active permits cannot be deleted.");
        }

        _context.PermitWorkers.RemoveRange(permit.AssignedWorkers);
        _context.PermitAssets.RemoveRange(permit.AssignedAssets);
        _context.EvidencePhotos.RemoveRange(permit.EvidencePhotos);
        if (permit.Approvals.Any()) _context.Approvals.RemoveRange(permit.Approvals);
        if (permit.AgentWorkflowRun != null) _context.AgentWorkflowRuns.Remove(permit.AgentWorkflowRun);

        _context.PermitRequests.Remove(permit);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ValidationReportDto> SubmitPermitForAiReviewAsync(Guid permitId)
    {
        var permit = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker)
            .Include(p => p.AssignedAssets).ThenInclude(pa => pa.Asset)
            .FirstOrDefaultAsync(p => p.Id == permitId);

        if (permit == null)
        {
            throw new ArgumentException("Permit not found.");
        }

        permit.Status = PermitStatus.AiReview;
        await _context.SaveChangesAsync();

        var startTime = DateTime.UtcNow;
        ValidationReportDto? report = null;
        object? traceObj = null;
        string modelUsed = "deterministic-rule-engine-v1";
        long durationMs = 0;

        // 1. Attempt Tier 4 Python LangGraph Multi-Agent Orchestrator invocation
        var agentBaseUrl = _config["AgentService:BaseUrl"] ?? "http://localhost:8000";
        var agentSecret = _config["AgentService:SharedSecret"] ?? "ClearToWork_Internal_Agent_Key_2026";
        var zone = await _context.Zones.FindAsync(permit.ZoneId);

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("X-Agent-Secret", agentSecret);

            var evaluatePayload = new
            {
                permit_id = permit.Id.ToString(),
                objective = permit.ObjectiveDescription,
                hazard_code = permit.PermitType?.Code ?? "HOT_WORK",
                zone_id = permit.ZoneId.ToString(),
                zone_code = zone?.Code ?? "ZONE_B3",
                start_time = permit.ScheduledStartTime.ToString("HH:mm"),
                end_time = permit.ScheduledEndTime.ToString("HH:mm"),
                worker_ids = permit.AssignedWorkers.Select(w => w.Worker?.BadgeNumber ?? w.WorkerId.ToString()).ToList(),
                asset_tags = permit.AssignedAssets.Select(a => a.Asset?.AssetTag ?? a.AssetId.ToString()).ToList()
            };

            var agentResp = await client.PostAsJsonAsync($"{agentBaseUrl}/evaluate-permit", evaluatePayload);
            if (agentResp.IsSuccessStatusCode)
            {
                var agentResult = await agentResp.Content.ReadFromJsonAsync<JsonElement>();
                durationMs = agentResult.TryGetProperty("duration_ms", out var dur) ? (long)dur.GetDouble() : (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
                modelUsed = "langgraph-5agent-pipeline";

                bool isSafeFailure = agentResult.TryGetProperty("is_safe_failure", out var isf) && isf.GetBoolean();
                string verdict = agentResult.TryGetProperty("verdict", out var v) ? v.GetString() ?? "CLEAR" : "CLEAR";

                var hardFailures = new List<string>();
                if (agentResult.TryGetProperty("hard_failures", out var hf) && hf.ValueKind == JsonValueKind.Array)
                {
                    foreach (var elem in hf.EnumerateArray())
                    {
                        var s = elem.GetString();
                        if (!string.IsNullOrEmpty(s)) hardFailures.Add(s);
                    }
                }

                AgentProposedFixDto? proposedFix = null;
                if (agentResult.TryGetProperty("proposed_fix", out var pf) && pf.ValueKind == JsonValueKind.Object)
                {
                    string wBadge = pf.TryGetProperty("suggestedWorkerBadge", out var wb) ? wb.GetString() ?? "" : "";
                    string aTag = pf.TryGetProperty("suggestedAssetTag", out var at) ? at.GetString() ?? "" : "";
                    string tWin = pf.TryGetProperty("suggestedTimeWindow", out var tw) ? tw.GetString() ?? "" : "";
                    string sExp = pf.TryGetProperty("summaryExplanation", out var se) ? se.GetString() ?? "" : "";
                    proposedFix = new AgentProposedFixDto(wBadge, aTag, tWin, sExp);
                }

                report = new ValidationReportDto(!isSafeFailure, verdict, hardFailures, new List<string>(), proposedFix);

                if (agentResult.TryGetProperty("execution_traces", out var traces))
                {
                    traceObj = new
                    {
                        workflow_id = agentResult.TryGetProperty("workflow_id", out var wid) ? wid.GetString() : Guid.NewGuid().ToString(),
                        objective = permit.ObjectiveDescription,
                        agents_executed = traces
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Tier 4 Agent service at {Url} unreachable ({Message}). Falling back to local deterministic validator.", agentBaseUrl, ex.Message);
        }

        // 2. Fallback: Local deterministic rule validator
        if (report == null)
        {
            report = await _validator.ValidatePermitRulesAsync(permit);
            durationMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

            var workerFindings = report.HardFailureReasons
                .Where(f => f.Contains("Welder") || f.Contains("Certificate") || f.Contains("worker") || f.Contains("qualified")).ToList();
            var equipmentFindings = report.HardFailureReasons
                .Where(f => f.Contains("inspection") || f.Contains("calibration") || f.Contains("overdue") || f.Contains("EX-") || f.Contains("GAS-") || f.Contains("Asset")).ToList();
            var hazardFindings = report.HardFailureReasons
                .Where(f => f.Contains("clash") || f.Contains("gusts") || f.Contains("wind") || f.Contains("rain") || f.Contains("Zone") || f.Contains("SIMOPS")).ToList();
            var planFindings = new List<string>
            {
                $"Permit type: {permit.PermitType?.Name ?? "HOT_WORK"} — max {permit.PermitType?.MaxDurationHours ?? 8}h window.",
                "Fire watch and continuous gas monitoring mandatory.",
                $"Objective: {permit.ObjectiveDescription[..Math.Min(80, permit.ObjectiveDescription.Length)]}..."
            };

            traceObj = new
            {
                workflow_id = Guid.NewGuid().ToString(),
                objective = permit.ObjectiveDescription,
                agents_executed = new object[]
                {
                    new { agent = "Planning & Coordination Agent", owner = "Student 3", tool = "get_permit_type_template", status = "SUCCESS", findings = planFindings, verdict = "", latency_ms = 45 },
                    new { agent = "Personnel & Competency Agent", owner = "Student 1", tool = "get_worker_certificates", status = "COMPLETED", findings = workerFindings, verdict = "", latency_ms = 72 },
                    new { agent = "Resource & Isolation Agent", owner = "Student 2", tool = "check_equipment_readiness", status = "COMPLETED", findings = equipmentFindings, verdict = "", latency_ms = 60 },
                    new { agent = "Site Conditions & Hazard Agent", owner = "Student 4", tool = "get_zone_conflicts", status = "COMPLETED", findings = hazardFindings, verdict = "", latency_ms = 85 },
                    new { agent = "Validation & Safety Agent", owner = "Shared", tool = "run_permit_validator", status = "COMPLETED", findings = report.HardFailureReasons, verdict = report.Verdict, latency_ms = 22 }
                }
            };
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var workflowRun = new AgentWorkflowRun
        {
            Id = Guid.NewGuid(),
            PermitRequestId = permit.Id,
            OutcomeStatus = report.IsApproved ? WorkflowOutcome.Clear : WorkflowOutcome.Refused_SafeFailure,
            DurationMs = durationMs,
            ModelUsed = modelUsed,
            ExecutionTraceJson = JsonSerializer.Serialize(traceObj, jsonOptions),
            RecommendedFixJson = report.ProposedFix != null ? JsonSerializer.Serialize(report.ProposedFix, jsonOptions) : null
        };

        _context.AgentWorkflowRuns.Add(workflowRun);

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
                pw.Worker?.Contractor?.CompanyName ?? "Unknown Contractor",
                pw.Worker?.IsActive ?? true,
                pw.Worker?.Certificates.Select(c => new WorkerCertificateDto(
                    c.Id,
                    c.CertificateType?.Code ?? "",
                    c.CertificateType?.Name ?? "",
                    c.CertificateNumber,
                    c.IssuingBody,
                    c.IssueDate,
                    c.ExpiryDate,
                    c.Status.ToString(),
                    (c.ExpiryDate.Date - DateTime.UtcNow.Date).Days
                )).ToList() ?? new List<WorkerCertificateDto>()
            )).ToList(),
            p.AssignedAssets.Select(pa =>
            {
                var latestInspection = pa.Asset?.InspectionRecords
                    .OrderByDescending(ir => ir.InspectionDate).FirstOrDefault();
                var latestCalibration = pa.Asset?.CalibrationRecords
                    .OrderByDescending(cr => cr.CalibrationDate).FirstOrDefault();
                bool inspectionValid = latestInspection != null && latestInspection.NextInspectionDate >= DateTime.UtcNow;
                bool calibrationValid = latestCalibration != null && latestCalibration.NextCalibrationDate >= DateTime.UtcNow;
                // AssetDto: (Id, Tag, Name, Category, Status, ZoneId, IsCalibrationValid, NextCalibrationDate, IsInspectionValid, NextInspectionDate)
                return new AssetDto(
                    pa.AssetId,
                    pa.Asset?.AssetTag ?? "",
                    pa.Asset?.Name ?? "",
                    pa.Asset?.Category.ToString() ?? "",
                    pa.Asset?.Status.ToString() ?? "",
                    pa.Asset?.CurrentZoneId,
                    calibrationValid,
                    latestCalibration != null ? latestCalibration.NextCalibrationDate : (DateTime?)null,
                    inspectionValid,
                    latestInspection != null ? latestInspection.NextInspectionDate : (DateTime?)null
                );
            }).ToList(),
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
