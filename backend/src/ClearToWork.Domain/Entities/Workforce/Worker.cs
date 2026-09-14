using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Workforce;

public class Worker : BaseAuditableEntity
{
    public string BadgeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Trade { get; set; } = string.Empty;
    public Guid ContractorId { get; set; }
    public Contractor? Contractor { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<WorkerCertificate> Certificates { get; set; } = new List<WorkerCertificate>();
    public ICollection<TrainingRecord> TrainingRecords { get; set; } = new List<TrainingRecord>();
}