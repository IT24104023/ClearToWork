using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly bool _isNpgsql;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        _isNpgsql = options.Extensions.Any(e => e.GetType().Name.Contains("Npgsql", StringComparison.OrdinalIgnoreCase));
    }

    // Shared
    public DbSet<User> Users => Set<User>();

    // Student 1: Workforce Competency
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<CertificateType> CertificateTypes => Set<CertificateType>();
    public DbSet<WorkerCertificate> WorkerCertificates => Set<WorkerCertificate>();
    public DbSet<TrainingRecord> TrainingRecords => Set<TrainingRecord>();

    // Student 2: Equipment & Isolation
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<CalibrationRecord> CalibrationRecords => Set<CalibrationRecord>();
    public DbSet<InspectionRecord> InspectionRecords => Set<InspectionRecord>();
    public DbSet<IsolationPoint> IsolationPoints => Set<IsolationPoint>();
    public DbSet<AssetQrTag> AssetQrTags => Set<AssetQrTag>();

    // Student 3: Permit Lifecycle
    public DbSet<PermitType> PermitTypes => Set<PermitType>();
    public DbSet<PermitRequest> PermitRequests => Set<PermitRequest>();
    public DbSet<PermitWorker> PermitWorkers => Set<PermitWorker>();
    public DbSet<PermitAsset> PermitAssets => Set<PermitAsset>();
    public DbSet<EvidencePhoto> EvidencePhotos => Set<EvidencePhoto>();
    public DbSet<Approval> Approvals => Set<Approval>();
    public DbSet<CloseOut> CloseOuts => Set<CloseOut>();
    public DbSet<AgentWorkflowRun> AgentWorkflowRuns => Set<AgentWorkflowRun>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    // Student 4: Hazards, Zones & Rulebook
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<ZoneAdjacency> ZoneAdjacencies => Set<ZoneAdjacency>();
    public DbSet<HazardType> HazardTypes => Set<HazardType>();
    public DbSet<ControlMeasure> ControlMeasures => Set<ControlMeasure>();
    public DbSet<IncompatibilityRule> IncompatibilityRules => Set<IncompatibilityRule>();
    public DbSet<Observation> Observations => Set<Observation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Indexes for performance and constraints
        modelBuilder.Entity<Worker>()
            .HasIndex(w => w.BadgeNumber)
            .IsUnique();

        modelBuilder.Entity<WorkerCertificate>()
            .HasIndex(wc => wc.ExpiryDate);

        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.AssetTag)
            .IsUnique();

        modelBuilder.Entity<AssetQrTag>()
            .HasIndex(t => t.QrCodeValue)
            .IsUnique();

        modelBuilder.Entity<PermitRequest>()
            .HasIndex(p => p.PermitNumber)
            .IsUnique();

        modelBuilder.Entity<PermitRequest>()
            .HasIndex(p => new { p.ZoneId, p.ScheduledStartTime, p.ScheduledEndTime });

        modelBuilder.Entity<Zone>()
            .HasIndex(z => z.Code)
            .IsUnique();

        // Join Entities
        modelBuilder.Entity<PermitWorker>()
            .HasKey(pw => new { pw.PermitRequestId, pw.WorkerId });

        modelBuilder.Entity<PermitAsset>()
            .HasKey(pa => new { pa.PermitRequestId, pa.AssetId });

        modelBuilder.Entity<ZoneAdjacency>()
            .HasKey(za => new { za.ZoneId, za.AdjacentZoneId });

        modelBuilder.Entity<ZoneAdjacency>()
            .HasOne(za => za.Zone)
            .WithMany(z => z.AdjacentZones)
            .HasForeignKey(za => za.ZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ZoneAdjacency>()
            .HasOne(za => za.AdjacentZone)
            .WithMany()
            .HasForeignKey(za => za.AdjacentZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // PostgreSQL JSONB configuration
        if (_isNpgsql)
        {
            modelBuilder.Entity<AgentWorkflowRun>()
                .Property(a => a.ExecutionTraceJson)
                .HasColumnType("jsonb");

            modelBuilder.Entity<AgentWorkflowRun>()
                .Property(a => a.RecommendedFixJson)
                .HasColumnType("jsonb");

            modelBuilder.Entity<AuditEntry>()
                .Property(a => a.ChangesJson)
                .HasColumnType("jsonb");
        }
    }
}
