using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ClearToWork.Tests.Workforce;

/// <summary>
/// Unit tests for the <see cref="Worker"/> domain entity.
/// Validates entity construction, required fields, certification associations,
/// and edge cases like deactivation and multiple certificate lifecycle states.
/// </summary>
public class WorkerEntityTests
{
    [Fact]
    public void CreateWorker_WithValidDetails_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var workerId = Guid.NewGuid();
        var contractorId = Guid.NewGuid();
        const string badgeNumber = "W-1042";
        const string firstName = "Dinithi";
        const string lastName = "Fernando";
        const string trade = "Instrument Technician";

        // Act
        var worker = new Worker
        {
            Id = workerId,
            BadgeNumber = badgeNumber,
            FirstName = firstName,
            LastName = lastName,
            Trade = trade,
            ContractorId = contractorId,
            IsActive = true
        };

        // Assert
        worker.Should().NotBeNull();
        worker.Id.Should().Be(workerId);
        worker.BadgeNumber.Should().Be(badgeNumber);
        worker.FirstName.Should().Be(firstName);
        worker.LastName.Should().Be(lastName);
        worker.Trade.Should().Be(trade);
        worker.ContractorId.Should().Be(contractorId);
        worker.IsActive.Should().BeTrue();
        worker.Certificates.Should().NotBeNull().And.BeEmpty();
        worker.TrainingRecords.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData("", "Fernando", "Welder")]
    [InlineData("   ", "Fernando", "Welder")]
    [InlineData("Dinithi", "", "Welder")]
    [InlineData("Dinithi", "   ", "Welder")]
    [InlineData(null, "Fernando", "Welder")]
    [InlineData("Dinithi", null, "Welder")]
    public void ValidateRequiredFields_WhenNameIsMissingOrWhitespace_ShouldFailValidation(
        string? firstName,
        string? lastName,
        string trade)
    {
        // Arrange
        var worker = new Worker
        {
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            Trade = trade,
            ContractorId = Guid.NewGuid()
        };

        // Act
        var validationErrors = ValidateWorker(worker);

        // Assert
        validationErrors.Should().NotBeEmpty();
        validationErrors.Should().Contain(err => err.Contains("Name", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ValidateRequiredFields_WhenTradeIsMissingOrWhitespace_ShouldFailValidation(string? invalidTrade)
    {
        // Arrange
        var worker = new Worker
        {
            FirstName = "Dinithi",
            LastName = "Fernando",
            Trade = invalidTrade ?? string.Empty,
            ContractorId = Guid.NewGuid()
        };

        // Act
        var validationErrors = ValidateWorker(worker);

        // Assert
        validationErrors.Should().NotBeEmpty();
        validationErrors.Should().Contain(err => err.Contains("Trade", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateRequiredFields_WhenCompanyIdIsEmpty_ShouldFailValidation()
    {
        // Arrange
        var worker = new Worker
        {
            FirstName = "Dinithi",
            LastName = "Fernando",
            Trade = "Scaffolder",
            ContractorId = Guid.Empty // Invalid: Company/Contractor ID cannot be empty
        };

        // Act
        var validationErrors = ValidateWorker(worker);

        // Assert
        validationErrors.Should().NotBeEmpty();
        validationErrors.Should().Contain(err => err.Contains("Company", StringComparison.OrdinalIgnoreCase) ||
                                                 err.Contains("Contractor", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AssignCertificate_ToWorker_ShouldAddCertificateToCollection()
    {
        // Arrange
        var worker = new Worker
        {
            Id = Guid.NewGuid(),
            FirstName = "Dinithi",
            LastName = "Fernando",
            Trade = "Electrician",
            ContractorId = Guid.NewGuid()
        };

        var certTypeId = Guid.NewGuid();
        var certificate = new WorkerCertificate
        {
            Id = Guid.NewGuid(),
            WorkerId = worker.Id,
            CertificateTypeId = certTypeId,
            CertificateNumber = "CERT-ELEC-2026-09",
            IssuingBody = "OPITO / City & Guilds",
            IssueDate = DateTime.UtcNow.AddMonths(-6),
            ExpiryDate = DateTime.UtcNow.AddMonths(18),
            Status = CertificateStatus.Valid
        };

        // Act
        worker.Certificates.Add(certificate);

        // Assert
        worker.Certificates.Should().ContainSingle();
        var assignedCert = worker.Certificates.First();
        assignedCert.CertificateNumber.Should().Be("CERT-ELEC-2026-09");
        assignedCert.WorkerId.Should().Be(worker.Id);
        assignedCert.Status.Should().Be(CertificateStatus.Valid);
        assignedCert.ExpiryDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void AssignMultipleCertificates_WithDifferentStatuses_ShouldCorrectlyReflectInWorkerProfile()
    {
        // Arrange
        var worker = new Worker
        {
            Id = Guid.NewGuid(),
            FirstName = "Dinithi",
            LastName = "Fernando",
            Trade = "Pipefitter",
            ContractorId = Guid.NewGuid()
        };

        var validCert = new WorkerCertificate
        {
            Id = Guid.NewGuid(),
            WorkerId = worker.Id,
            CertificateTypeId = Guid.NewGuid(),
            CertificateNumber = "CERT-VALID-001",
            Status = CertificateStatus.Valid,
            ExpiryDate = DateTime.UtcNow.AddDays(120)
        };

        var expiredCert = new WorkerCertificate
        {
            Id = Guid.NewGuid(),
            WorkerId = worker.Id,
            CertificateTypeId = Guid.NewGuid(),
            CertificateNumber = "CERT-EXP-002",
            Status = CertificateStatus.Expired,
            ExpiryDate = DateTime.UtcNow.AddDays(-15)
        };

        var suspendedCert = new WorkerCertificate
        {
            Id = Guid.NewGuid(),
            WorkerId = worker.Id,
            CertificateTypeId = Guid.NewGuid(),
            CertificateNumber = "CERT-SUSP-003",
            Status = CertificateStatus.Suspended,
            ExpiryDate = DateTime.UtcNow.AddDays(45)
        };

        // Act
        worker.Certificates.Add(validCert);
        worker.Certificates.Add(expiredCert);
        worker.Certificates.Add(suspendedCert);

        // Assert
        worker.Certificates.Should().HaveCount(3);
        worker.Certificates.Count(c => c.Status == CertificateStatus.Valid).Should().Be(1);
        worker.Certificates.Count(c => c.Status == CertificateStatus.Expired).Should().Be(1);
        worker.Certificates.Count(c => c.Status == CertificateStatus.Suspended).Should().Be(1);
    }

    [Fact]
    public void DeactivateWorker_ShouldSetIsActiveToFalse_WhilePreservingCertificatesAndHistory()
    {
        // Arrange
        var worker = new Worker
        {
            Id = Guid.NewGuid(),
            FirstName = "Dinithi",
            LastName = "Fernando",
            Trade = "Rigger",
            ContractorId = Guid.NewGuid(),
            IsActive = true
        };

        worker.Certificates.Add(new WorkerCertificate
        {
            Id = Guid.NewGuid(),
            WorkerId = worker.Id,
            CertificateNumber = "RIG-101",
            Status = CertificateStatus.Valid,
            ExpiryDate = DateTime.UtcNow.AddYears(1)
        });

        // Act - Soft deactivate worker (e.g. demobilized from site or leave of absence)
        worker.IsActive = false;

        // Assert - Invariants must hold: history is retained for auditing
        worker.IsActive.Should().BeFalse();
        worker.Certificates.Should().NotBeEmpty();
        worker.Certificates.Should().ContainSingle();
        worker.Certificates.First().CertificateNumber.Should().Be("RIG-101");
    }

    /// <summary>
    /// Helper method simulating the domain validation rules for Worker entity.
    /// Checks required Name (FirstName, LastName), Trade, and CompanyId (ContractorId).
    /// </summary>
    private static List<string> ValidateWorker(Worker worker)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(worker.FirstName) || string.IsNullOrWhiteSpace(worker.LastName))
        {
            errors.Add("Worker Name (both FirstName and LastName) is required.");
        }

        if (string.IsNullOrWhiteSpace(worker.Trade))
        {
            errors.Add("Worker Trade is required.");
        }

        if (worker.ContractorId == Guid.Empty)
        {
            errors.Add("Worker Contractor/Company ID is required.");
        }

        return errors;
    }
}
