using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Equipment;

public class CalibrationRecord : BaseAuditableEntity
{
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
    public DateTime CalibrationDate { get; set; }
    public DateTime NextCalibrationDate { get; set; }
    public string CalibratedBy { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public bool PassStatus { get; set; } = true;
}
