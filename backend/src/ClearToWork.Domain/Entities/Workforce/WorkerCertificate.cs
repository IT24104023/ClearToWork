using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;

namespace ClearToWork.Domain.Entities.Workforce;

public class WorkerCertificate : BaseAuditableEntity
{
    public Guid WorkerId { get; set; }
    public Worker? Worker { get; set; }
    public Guid CertificateTypeId { get; set; }
    public CertificateType? CertificateType { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string IssuingBody { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public CertificateStatus Status { get; set; } = CertificateStatus.Valid;
    public string? DocumentScanUrl { get; set; }
}
