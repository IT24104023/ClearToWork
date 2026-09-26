using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitType : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxDurationHours { get; set; } = 8;
    public bool RequiresFireWatch { get; set; } = false;
    /// <summary>Whether continuous gas monitoring (LEL/O2) is required before work commences.</summary>
    public bool RequiresGasTesting { get; set; } = false;
    /// <summary>JSON array of mandatory safety controls for this permit type, e.g. ["Dry powder extinguisher within 5m", "Continuous gas monitoring"].</summary>
    public string? MandatoryControlsJson { get; set; }
}