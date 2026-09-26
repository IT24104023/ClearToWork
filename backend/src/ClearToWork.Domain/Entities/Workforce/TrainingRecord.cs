using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Workforce;

public class TrainingRecord : BaseAuditableEntity
{
    public Guid WorkerId { get; set; }
    public Worker? Worker { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateTime CompletedDate { get; set; }
    public decimal Score { get; set; }
    public string TrainerName { get; set; } = string.Empty;
}