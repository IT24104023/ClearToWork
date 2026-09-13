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
