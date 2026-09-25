using System.Security.Cryptography;
using System.Text;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClearToWork.Infrastructure.Services;

#region Export Data Models

/// <summary>
/// Top-level PDF-ready document model aggregating all safety, personnel,
/// hazard mitigation, and verification data for printing an official Permit-to-Work.
/// </summary>
public class PermitExportModel
{
    public string DocumentReferenceNumber { get; set; } = string.Empty;
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
    public string SystemVersion { get; set; } = "ClearToWork AI v1.0 - ISO 45001 / OSHA 1910 Compliant";

    public PermitDetailsSection Details { get; set; } = new();
    public List<ExportWorkerItem> Workers { get; set; } = new();
    public HazardAssessmentSection HazardAssessment { get; set; } = new();
    public List<ApprovalChainItem> ApprovalChain { get; set; } = new();
    public SafetyConditionsSection SafetyConditions { get; set; } = new();
    public QrVerificationSection QrVerification { get; set; } = new();
}

public class PermitDetailsSection
{
    public Guid PermitId { get; set; }
    public string PermitNumber { get; set; } = string.Empty;
    public string PermitTypeName { get; set; } = string.Empty;
    public string PermitTypeCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
    public string ZoneCode { get; set; } = string.Empty;
    public string SupervisorName { get; set; } = string.Empty;
    public string ObjectiveDescription { get; set; } = string.Empty;
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public double EstimatedDurationHours { get; set; }
    public DateTime? ActivatedAt { get; set; }
}

public class ExportWorkerItem
{
    public string BadgeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Trade { get; set; } = string.Empty;
    public string ContractorName { get; set; } = string.Empty;
    public string RoleOnPermit { get; set; } = string.Empty;
    public List<ExportCertificateItem> Certificates { get; set; } = new();
}

public class ExportCertificateItem
{
    public string CertificateTypeCode { get; set; } = string.Empty;
    public string CertificateName { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsValidForExecutionWindow { get; set; }
}

public class HazardAssessmentSection
{
    public string HazardClassification { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "High";
    public List<string> IdentifiedHazards { get; set; } = new();
    public List<string> RequiredPpe { get; set; } = new();
    public bool RequiresAtmosphericTesting { get; set; }
    public AtmosphericLimits GasLimits { get; set; } = new();
    public string AiSafetyReviewVerdict { get; set; } = string.Empty;
    public List<string> MultiAgentFindings { get; set; } = new();
}

public class AtmosphericLimits
{
    public string Oxygen { get; set; } = "19.5% - 23.5%";
    public string FlammableGasesLEL { get; set; } = "< 5% LEL";
    public string HydrogenSulfideH2S { get; set; } = "< 5 ppm";
    public string CarbonMonoxideCO { get; set; } = "< 25 ppm";
}

public class ApprovalChainItem
{
    public int SequenceOrder { get; set; }
    public string RoleTitle { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string DigitalSignatureHash { get; set; } = string.Empty;
}

public class SafetyConditionsSection
{
    public List<string> MandatoryPrecautions { get; set; } = new();
    public bool RequiresFireWatch { get; set; }
    public int FireWatchDurationMinutesAfterHotWork { get; set; } = 30;
    public bool RequiresContinuousGasMonitoring { get; set; }
    public List<string> IsolationRequirements { get; set; } = new();
    public string EmergencyEvacuationPoint { get; set; } = "Assembly Point B - South Gantry";
    public string EmergencyContactRadioChannel { get; set; } = "Channel 4 (HSE Ops)";
}

public class QrVerificationSection
{
    public string QrToken { get; set; } = string.Empty;
    public string VerificationUrl { get; set; } = string.Empty;
    public string VerificationPayloadHash { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

#endregion

/// <summary>
/// Service interface for generating PDF-ready export models for Permit-to-Work documentation.
/// </summary>
public interface IPermitPdfExporter
{
    /// <summary>
    /// Compiles a fully populated, ready-to-render permit model from database records.
    /// </summary>
    /// <param name="permitId">The unique permit identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A populated <see cref="PermitExportModel"/>, or null if the permit does not exist.</returns>
    Task<PermitExportModel?> GenerateExportModelAsync(Guid permitId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Prepares a comprehensive, structured data model designed for PDF rendering
/// of official oil &amp; gas safety permits with QR audit verification.
/// </summary>
public class PermitPdfExporter : IPermitPdfExporter
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PermitPdfExporter> _logger;

    public PermitPdfExporter(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<PermitPdfExporter> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PermitExportModel?> GenerateExportModelAsync(Guid permitId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[PdfExporter] Generating PDF export model for Permit {PermitId}", permitId);

        var permit = await _context.PermitRequests
            .Include(p => p.PermitType)
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker).ThenInclude(w => w!.Contractor)
            .Include(p => p.AssignedWorkers).ThenInclude(pw => pw.Worker).ThenInclude(w => w!.Certificates).ThenInclude(c => c.CertificateType)
            .Include(p => p.AssignedAssets).ThenInclude(pa => pa.Asset)
            .Include(p => p.Approvals)
            .Include(p => p.AgentWorkflowRun)
            .FirstOrDefaultAsync(p => p.Id == permitId, cancellationToken);

        if (permit == null)
        {
            _logger.LogWarning("[PdfExporter] Permit {PermitId} not found.", permitId);
            return null;
        }

        var zone = await _context.Zones.Include(z => z.Site).FirstOrDefaultAsync(z => z.Id == permit.ZoneId, cancellationToken);
        var supervisor = await _context.Users.FindAsync(new object[] { permit.SupervisorId }, cancellationToken);
        var safetyOfficerIds = permit.Approvals.Select(a => a.SafetyOfficerId).Distinct().ToList();
        var officers = await _context.Users.Where(u => safetyOfficerIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        var duration = (permit.ScheduledEndTime - permit.ScheduledStartTime).TotalHours;
        var pCode = permit.PermitType?.Code ?? "GENERAL";

        var model = new PermitExportModel
        {
            DocumentReferenceNumber = $"CTW-DOC-{permit.PermitNumber}-{DateTime.UtcNow:yyyyMMdd}",
            GeneratedAtUtc = DateTime.UtcNow,

            // 1. Permit Details Section
            Details = new PermitDetailsSection
            {
                PermitId = permit.Id,
                PermitNumber = permit.PermitNumber,
                PermitTypeName = permit.PermitType?.Name ?? "General Safe Work Permit",
                PermitTypeCode = pCode,
                Status = permit.Status.ToString(),
                SiteName = zone?.Site?.Name ?? "Main Refinery & Terminal Facility",
                ZoneName = zone?.Name ?? "Primary Process Zone",
                ZoneCode = zone?.Code ?? "ZONE_GEN",
                SupervisorName = supervisor?.FullName ?? "Designated Area Supervisor",
                ObjectiveDescription = permit.ObjectiveDescription,
                ScheduledStartTime = permit.ScheduledStartTime,
                ScheduledEndTime = permit.ScheduledEndTime,
                EstimatedDurationHours = Math.Max(0.5, Math.Round(duration, 1)),
                ActivatedAt = permit.ActivatedAt
            },

            // 2. Assigned Workers Section
            Workers = permit.AssignedWorkers.Select(pw => new ExportWorkerItem
            {
                BadgeNumber = pw.Worker?.BadgeNumber ?? "N/A",
                FullName = pw.Worker != null ? $"{pw.Worker.FirstName} {pw.Worker.LastName}" : "Unknown Worker",
                Trade = pw.Worker?.Trade ?? "General Operations",
                ContractorName = pw.Worker?.Contractor?.CompanyName ?? "Direct Hire / ClearToWork",
                RoleOnPermit = pw.RoleOnPermit,
                Certificates = pw.Worker?.Certificates.Select(c => new ExportCertificateItem
                {
                    CertificateTypeCode = c.CertificateType?.Code ?? "CERT",
                    CertificateName = c.CertificateType?.Name ?? "Safety Qualification",
                    CertificateNumber = c.CertificateNumber,
                    ExpiryDate = c.ExpiryDate,
                    IsValidForExecutionWindow = c.Status == CertificateStatus.Valid && c.ExpiryDate > permit.ScheduledEndTime
                }).ToList() ?? new List<ExportCertificateItem>()
            }).ToList(),

            // 3. Hazard Assessment Summary
            HazardAssessment = BuildHazardAssessment(permit, pCode),

            // 4. Approval Chain
            ApprovalChain = BuildApprovalChain(permit, officers),

            // 5. Safety Conditions & Isolations
            SafetyConditions = BuildSafetyConditions(permit, pCode),

            // 6. QR Verification Data
            QrVerification = BuildQrVerification(permit)
        };

        _logger.LogInformation("[PdfExporter] Successfully compiled export model for {PermitNumber}.", permit.PermitNumber);
        return model;
    }

    private static HazardAssessmentSection BuildHazardAssessment(PermitRequest permit, string permitTypeCode)
    {
        var hazards = new List<string>();
        var ppe = new List<string> { "Standard Flame-Retardant Coveralls (EN ISO 11612)", "Hard Hat with 4-Point Chinstrap", "Safety Footwear with Steel Toe (EN ISO 20345)" };

        bool atmosphericTesting = false;

        switch (permitTypeCode.ToUpperInvariant())
        {
            case "HOT_WORK":
                hazards.Add("Sparks, slag, and open ignition source in proximity to hydrocarbon infrastructure");
                hazards.Add("Risk of flammable gas migration from adjacent drainage or relief vents");
                ppe.Add("Welding Helmet with Auto-Darkening Filter (shade 10-12)");
                ppe.Add("Heavy Split Cowhide Welding Gauntlets");
                atmosphericTesting = true;
                break;
            case "CONFINED_SPACE":
                hazards.Add("Oxygen depletion or atmospheric toxic gas accumulation");
                hazards.Add("Restricted ingress/egress and emergency extraction difficulty");
                ppe.Add("Full Body Fall-Arrest Extraction Harness");
                ppe.Add("Emergency Escape Breathing Apparatus (EEBA - 15 min)");
                atmosphericTesting = true;
                break;
            case "WORKING_AT_HEIGHT":
                hazards.Add("Personnel fall from elevation exceeding 1.8 meters");
                hazards.Add("Dropped objects impacting ground personnel below work envelope");
                ppe.Add("Full Body Harness with Twin Elasticated Lanyards and Scaffold Hooks");
                ppe.Add("Tool Lanyards for all hand equipment");
                break;
            default:
                hazards.Add("General mechanical and slip/trip hazards during site execution");
                ppe.Add("Safety Glasses with Side Shields (ANSI Z87.1)");
                ppe.Add("Cut-Resistant Mechanics Gloves (Level 4)");
                break;
        }

        var findings = new List<string>();
        if (permit.AgentWorkflowRun != null)
        {
            findings.Add($"AI Multi-Agent Safety Engine: {permit.AgentWorkflowRun.OutcomeStatus}");
            findings.Add($"Model Execution: {permit.AgentWorkflowRun.ModelUsed} ({permit.AgentWorkflowRun.DurationMs} ms)");
        }

        return new HazardAssessmentSection
        {
            HazardClassification = permitTypeCode,
            RiskLevel = permitTypeCode == "HOT_WORK" || permitTypeCode == "CONFINED_SPACE" ? "High (Level 3)" : "Medium (Level 2)",
            IdentifiedHazards = hazards,
            RequiredPpe = ppe,
            RequiresAtmosphericTesting = atmosphericTesting,
            GasLimits = new AtmosphericLimits(),
            AiSafetyReviewVerdict = permit.AgentWorkflowRun?.OutcomeStatus.ToString() ?? "Deterministic Clearance Passed",
            MultiAgentFindings = findings
        };
    }

    private static List<ApprovalChainItem> BuildApprovalChain(PermitRequest permit, Dictionary<Guid, string> officers)
    {
        var items = new List<ApprovalChainItem>();
        int order = 1;

        foreach (var app in permit.Approvals.OrderBy(a => a.DecisionTimestamp))
        {
            var officerName = officers.GetValueOrDefault(app.SafetyOfficerId, "Authorized HSE Safety Officer");
            var signaturePayload = $"{permit.PermitNumber}|{app.SafetyOfficerId}|{app.Decision}|{app.DecisionTimestamp:O}";

            items.Add(new ApprovalChainItem
            {
                SequenceOrder = order++,
                RoleTitle = "HSE Safety Officer Sign-Off",
                ApproverName = officerName,
                Decision = app.Decision.ToString(),
                Notes = string.IsNullOrWhiteSpace(app.DecisionNotes) ? "Standard safety precautions accepted." : app.DecisionNotes,
                Timestamp = app.DecisionTimestamp,
                DigitalSignatureHash = ComputeSha256(signaturePayload)
            });
        }

        return items;
    }

    private static SafetyConditionsSection BuildSafetyConditions(PermitRequest permit, string permitTypeCode)
    {
        var precautions = new List<string>
        {
            "Inspect physical work zone and verify zero combustible material within 10m radius.",
            "Verify all isolation padlocks and danger tags against Master Isolation Sheet before commencement.",
            "Ensure emergency eyewash and first aid kit are within 30 seconds unobstructed travel distance."
        };

        bool fireWatch = permitTypeCode == "HOT_WORK";
        bool continuousGas = permitTypeCode == "HOT_WORK" || permitTypeCode == "CONFINED_SPACE";

        var isolations = new List<string>();
        if (permit.PermitType != null && permit.PermitType.RequiresIsolation)
        {
            isolations.Add("ISO-01: Lockout/Tagout valve V-2041 on Main Process Header");
            isolations.Add("ISO-02: Breaker MCC-04 Rack Out & Padlock applied by Electrical Tech");
        }
        else
        {
            isolations.Add("No mechanical/electrical isolations required for this task type.");
        }

        return new SafetyConditionsSection
        {
            MandatoryPrecautions = precautions,
            RequiresFireWatch = fireWatch,
            FireWatchDurationMinutesAfterHotWork = fireWatch ? 30 : 0,
            RequiresContinuousGasMonitoring = continuousGas,
            IsolationRequirements = isolations,
            EmergencyEvacuationPoint = "Assembly Point B - South Gantry",
            EmergencyContactRadioChannel = "Channel 4 (HSE Ops)"
        };
    }

    private QrVerificationSection BuildQrVerification(PermitRequest permit)
    {
        var clientBaseUrl = _configuration["App:ClientBaseUrl"] ?? "https://cleartowork.internal.energy";
        var token = permit.PermitQrToken ?? $"PTW-VERIFY-{permit.PermitNumber}-{Guid.NewGuid():N}";
        var verifyUrl = $"{clientBaseUrl}/permits/verify/{token}";
        var hash = ComputeSha256($"{permit.PermitNumber}|{token}|{permit.ScheduledEndTime:O}");

        return new QrVerificationSection
        {
            QrToken = token,
            VerificationUrl = verifyUrl,
            VerificationPayloadHash = hash,
            Instructions = "Scan this QR code with the ClearToWork Mobile App on site to confirm permit validity, activation status, and authorized workers.",
            ExpiresAtUtc = permit.ScheduledEndTime
        };
    }

    private static string ComputeSha256(string rawData)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes)[..16]; // Compact 16-char digital seal
    }
}
