using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Equipment;

public class InspectionRecord : BaseEntity
{
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
    public DateTime InspectionDate { get; set; } = DateTime.UtcNow;
    public DateTime NextInspectionDate { get; set; } = DateTime.UtcNow.AddMonths(3);
    public string InspectorName { get; set; } = string.Empty;
    public bool Passed { get; set; } = true;
    public string? Notes { get; set; }
}
