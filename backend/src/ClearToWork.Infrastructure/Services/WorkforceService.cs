using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;
using ClearToWork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClearToWork.Infrastructure.Services;

public class WorkforceService : IWorkforceService
{
    private readonly AppDbContext _context;

    public WorkforceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkerDto>> GetAllWorkersAsync(string? trade = null, bool? activeOnly = true)
    {
        var query = _context.Workers
            .Include(w => w.Contractor)
            .Include(w => w.Certificates)
                .ThenInclude(c => c.CertificateType)
            .AsQueryable();

        if (activeOnly == true)
        {
            query = query.Where(w => w.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(trade))
        {
            query = query.Where(w => w.Trade.ToLower() == trade.ToLower());
        }

        var workers = await query.ToListAsync();
        return workers.Select(MapWorkerToDto).ToList();
    }

    public async Task<WorkerDto?> GetWorkerByIdAsync(Guid id)
    {
        var worker = await _context.Workers
            .Include(w => w.Contractor)
            .Include(w => w.Certificates)
                .ThenInclude(c => c.CertificateType)
            .FirstOrDefaultAsync(w => w.Id == id);

        return worker == null ? null : MapWorkerToDto(worker);
    }

    public async Task<WorkerDto> CreateWorkerAsync(string firstName, string lastName, string badgeNumber, string trade, Guid contractorId)
    {
        var worker = new Worker
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            BadgeNumber = badgeNumber,
            Trade = trade,
            ContractorId = contractorId,
            IsActive = true
        };

        _context.Workers.Add(worker);
        await _context.SaveChangesAsync();
        return (await GetWorkerByIdAsync(worker.Id))!;
    }

    public async Task<WorkerDto?> UpdateWorkerAsync(Guid id, UpdateWorkerRequest request)
    {
        var worker = await _context.Workers
            .Include(w => w.Contractor)
            .Include(w => w.Certificates)
                .ThenInclude(c => c.CertificateType)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (worker == null) return null;

        worker.FirstName = request.FirstName.Trim();
        worker.LastName = request.LastName.Trim();
        worker.Trade = request.Trade.Trim();
        worker.IsActive = request.IsActive;
        if (request.ContractorId.HasValue && request.ContractorId.Value != Guid.Empty)
        {
            worker.ContractorId = request.ContractorId.Value;
        }

        await _context.SaveChangesAsync();
        return await GetWorkerByIdAsync(worker.Id);
    }

    public async Task<bool> DeleteWorkerAsync(Guid id)
    {
        var worker = await _context.Workers.FindAsync(id);
        if (worker == null) return false;

        // Remove certificates first
        var certs = await _context.WorkerCertificates.Where(c => c.WorkerId == id).ToListAsync();
        _context.WorkerCertificates.RemoveRange(certs);

        _context.Workers.Remove(worker);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WorkerCertificateDto?> AddWorkerCertificateAsync(Guid workerId, CreateCertificateRequest request)
    {
        var worker = await _context.Workers.FindAsync(workerId);
        if (worker == null) return null;

        var certType = await _context.CertificateTypes.FindAsync(request.CertificateTypeId);

        var cert = new WorkerCertificate
        {
            Id = Guid.NewGuid(),
            WorkerId = workerId,
            CertificateTypeId = request.CertificateTypeId,
            CertificateNumber = request.CertificateNumber.Trim(),
            IssuingBody = request.IssuingBody.Trim(),
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            Status = request.ExpiryDate > DateTime.UtcNow ? CertificateStatus.Valid : CertificateStatus.Expired
        };

        _context.WorkerCertificates.Add(cert);
        await _context.SaveChangesAsync();

        return new WorkerCertificateDto(
            cert.Id,
            certType?.Code ?? "",
            certType?.Name ?? "",
            cert.CertificateNumber,
            cert.IssuingBody,
            cert.IssueDate,
            cert.ExpiryDate,
            cert.Status.ToString(),
            (cert.ExpiryDate.Date - DateTime.UtcNow.Date).Days
        );
    }

    public async Task<bool> DeleteWorkerCertificateAsync(Guid certificateId)
    {
        var cert = await _context.WorkerCertificates.FindAsync(certificateId);
        if (cert == null) return false;

        _context.WorkerCertificates.Remove(cert);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ContractorDto>> GetContractorsAsync()
    {
        return await _context.Contractors
            .OrderBy(c => c.CompanyName)
            .Select(c => new ContractorDto(c.Id, c.CompanyName, c.LicenseNumber, c.ContactEmail))
            .ToListAsync();
    }

    public async Task<List<CertificateTypeDto>> GetCertificateTypesAsync()
    {
        return await _context.CertificateTypes
            .OrderBy(ct => ct.Name)
            .Select(ct => new CertificateTypeDto(ct.Id, ct.Code, ct.Name, ct.RequiredForTrade, ct.ValidityMonths))
            .ToListAsync();
    }

    public async Task<EligibilityCheckResponse> CheckEligibilityAsync(EligibilityCheckRequest request)
    {
        var workers = await _context.Workers
            .Include(w => w.Certificates)
                .ThenInclude(c => c.CertificateType)
            .Where(w => request.WorkerIds.Contains(w.Id))
            .ToListAsync();

        var results = new List<WorkerEligibilityResult>();
        bool allEligible = true;

        foreach (var worker in workers)
        {
            var missingOrExpired = new List<string>();
            int? minDaysToExpiry = null;

            // Relevant certificate check based on hazard
            var certs = worker.Certificates.ToList();
            if (!certs.Any())
            {
                missingOrExpired.Add($"No certifications on record for worker {worker.BadgeNumber}.");
            }
            else
            {
                foreach (var cert in certs)
                {
                    int daysRemaining = (cert.ExpiryDate.Date - request.JobDate.Date).Days;
                    if (minDaysToExpiry == null || daysRemaining < minDaysToExpiry)
                    {
                        minDaysToExpiry = daysRemaining;
                    }

                    if (cert.ExpiryDate < request.JobDate)
                    {
                        missingOrExpired.Add($"Certificate {cert.CertificateType?.Name ?? cert.CertificateNumber} expired on {cert.ExpiryDate:yyyy-MM-dd} ({-daysRemaining} days ago).");
                    }
                    else if (cert.Status == CertificateStatus.Suspended)
                    {
                        missingOrExpired.Add($"Certificate {cert.CertificateNumber} is suspended.");
                    }
                }
            }

            bool eligible = missingOrExpired.Count == 0;
            if (!eligible) allEligible = false;

            results.Add(new WorkerEligibilityResult(
                worker.Id,
                worker.BadgeNumber,
                $"{worker.FirstName} {worker.LastName}",
                eligible,
                missingOrExpired,
                minDaysToExpiry
            ));
        }

        // Recommend qualified replacements if any worker is ineligible
        var replacements = new List<WorkerDto>();
        if (!allEligible)
        {
            var eligibleAlternatives = await _context.Workers
                .Include(w => w.Contractor)
                .Include(w => w.Certificates)
                    .ThenInclude(c => c.CertificateType)
                .Where(w => !request.WorkerIds.Contains(w.Id) && w.IsActive)
                .Where(w => w.Certificates.Any(c => c.ExpiryDate > request.JobDate && c.Status == CertificateStatus.Valid))
                .Take(3)
                .ToListAsync();

            replacements = eligibleAlternatives.Select(MapWorkerToDto).ToList();
        }

        return new EligibilityCheckResponse(allEligible, results, replacements);
    }

    public async Task<List<ExpiryForecastItem>> Get30DayExpiryForecastAsync()
    {
        var targetDate = DateTime.UtcNow.AddDays(30);
        var certs = await _context.WorkerCertificates
            .Include(c => c.Worker)
            .Include(c => c.CertificateType)
            .Where(c => c.ExpiryDate <= targetDate && c.ExpiryDate >= DateTime.UtcNow.AddDays(-30))
            .OrderBy(c => c.ExpiryDate)
            .ToListAsync();

        return certs.Select(c => new ExpiryForecastItem(
            c.WorkerId,
            $"{c.Worker?.FirstName} {c.Worker?.LastName}",
            c.Worker?.BadgeNumber ?? "Unknown",
            c.CertificateType?.Name ?? c.CertificateNumber,
            c.ExpiryDate,
            (c.ExpiryDate.Date - DateTime.UtcNow.Date).Days
        )).ToList();
    }

    private static WorkerDto MapWorkerToDto(Worker w)
    {
        return new WorkerDto(
            w.Id,
            w.BadgeNumber,
            w.FirstName,
            w.LastName,
            w.Trade,
            w.ContractorId,
            w.Contractor?.CompanyName ?? "Unknown",
            w.IsActive,
            w.Certificates.Select(c => new WorkerCertificateDto(
                c.Id,
                c.CertificateType?.Code ?? "",
                c.CertificateType?.Name ?? "",
                c.CertificateNumber,
                c.IssuingBody,
                c.IssueDate,
                c.ExpiryDate,
                c.Status.ToString(),
                (c.ExpiryDate.Date - DateTime.UtcNow.Date).Days
            )).ToList()
        );
    }
}
