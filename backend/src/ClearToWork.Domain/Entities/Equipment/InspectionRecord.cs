using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Equipment;

public class InspectionRecord : BaseAuditableEntity
{
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
    public DateTime InspectionDate { get; set; }
    public DateTime NextInspectionDate { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public bool Passed { get; set; } = true;
    public string? Notes { get; set; }
}
