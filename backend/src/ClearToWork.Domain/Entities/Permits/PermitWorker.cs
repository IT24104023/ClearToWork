using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitWorker : BaseEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public Guid WorkerId { get; set; }
    public Worker? Worker { get; set; }
    public string RoleOnPermit { get; set; } = "Lead";
}
