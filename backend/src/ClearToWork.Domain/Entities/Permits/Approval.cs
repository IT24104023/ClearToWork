using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;

namespace ClearToWork.Domain.Entities.Permits;

public class Approval : BaseEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public Guid SafetyOfficerId { get; set; }
    public string SafetyOfficerName { get; set; } = string.Empty;
    public DecisionType Decision { get; set; }
    public string DecisionNotes { get; set; } = string.Empty;
    public DateTime DecisionTimestamp { get; set; } = DateTime.UtcNow;
}
