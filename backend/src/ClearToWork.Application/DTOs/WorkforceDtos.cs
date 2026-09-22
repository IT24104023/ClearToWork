namespace ClearToWork.Application.DTOs;

public record WorkerDto(
    Guid Id,
    string BadgeNumber,
    string FirstName,
    string LastName,
    string Trade,
    Guid ContractorId,
    string ContractorName,
    bool IsActive,
    List<WorkerCertificateDto> Certificates
);

public record WorkerCertificateDto(
    Guid Id,
    string CertificateCode,
    string CertificateName,
    string CertificateNumber,
    string IssuingBody,
    DateTime IssueDate,
    DateTime ExpiryDate,
    string Status,
    int DaysUntilExpiry
);

public record EligibilityCheckRequest(
    List<Guid> WorkerIds,
    string HazardCode,
    DateTime JobDate
);

public record WorkerEligibilityResult(
    Guid WorkerId,
    string BadgeNumber,
    string FullName,
    bool IsEligible,
    List<string> MissingOrExpiredCertificates,
    int? MinDaysToExpiry
);

public record EligibilityCheckResponse(
    bool AllEligible,
    List<WorkerEligibilityResult> Results,
    List<WorkerDto> RecommendedReplacements
);

public record ExpiryForecastItem(
    Guid WorkerId,
    string WorkerName,
    string BadgeNumber,
    string CertificateName,
    DateTime ExpiryDate,
    int DaysRemaining
);

public record CreateWorkerRequest(
    string FirstName,
    string LastName,
    string BadgeNumber,
    string Trade,
    Guid ContractorId
);

public record UpdateWorkerRequest(
    string FirstName,
    string LastName,
    string Trade,
    bool IsActive,
    Guid? ContractorId = null
);

public record CreateCertificateRequest(
    Guid CertificateTypeId,
    string CertificateNumber,
    string IssuingBody,
    DateTime IssueDate,
    DateTime ExpiryDate
);

public record ContractorDto(
    Guid Id,
    string CompanyName,
    string LicenseNumber,
    string ContactEmail
);

public record CertificateTypeDto(
    Guid Id,
    string Code,
    string Name,
    string RequiredForTrade,
    int ValidityMonths
);
