using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitAsset : BaseEntity
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
    public DateTime ReservedFrom { get; set; }
    public DateTime ReservedUntil { get; set; }
    public DateTime? ReturnedAt { get; set; }
}
