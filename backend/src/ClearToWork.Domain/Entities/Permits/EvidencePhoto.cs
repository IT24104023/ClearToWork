using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Permits;

public class EvidencePhoto : BaseEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public string Stage { get; set; } = "Submission";
    public string PhotoUrl { get; set; } = string.Empty;
    public decimal? GpsLatitude { get; set; }
    public decimal? GpsLongitude { get; set; }
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}