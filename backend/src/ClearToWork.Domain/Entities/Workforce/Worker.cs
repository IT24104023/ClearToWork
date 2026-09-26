using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Workforce;

public class Worker : BaseAuditableEntity
{
    public string BadgeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Trade { get; set; } = string.Empty;
    public Guid ContractorId { get; set; }
    public Contractor? Contractor { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<WorkerCertificate> Certificates { get; set; } = new List<WorkerCertificate>();
    public ICollection<TrainingRecord> TrainingRecords { get; set; } = new List<TrainingRecord>();
}