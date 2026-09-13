using ClearToWork.Domain.Common;

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
