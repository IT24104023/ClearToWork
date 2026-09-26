using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Equipment;

public class CalibrationRecord : BaseEntity
{
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
    public DateTime CalibrationDate { get; set; } = DateTime.UtcNow;
    public DateTime NextCalibrationDate { get; set; } = DateTime.UtcNow.AddMonths(6);
    public string CalibratedBy { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public bool PassStatus { get; set; } = true;
}