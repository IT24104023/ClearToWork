using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitRequest : BaseAuditableEntity
{
    public string PermitNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid PermitTypeId { get; set; }
    public PermitType? PermitType { get; set; }
    public string Status { get; set; } = "Draft";
    public string ZoneCode { get; set; } = "ZONE-A1";
    public Guid ZoneId { get; set; }
    public string IssuingAuthority { get; set; } = "Mohammed Zakee";
    public DateTime ScheduledStartTime { get; set; } = DateTime.UtcNow;
    public DateTime ScheduledEndTime { get; set; } = DateTime.UtcNow.AddHours(12);
    public ICollection<PermitWorker> AssignedWorkers { get; set; } = new List<PermitWorker>();
}
