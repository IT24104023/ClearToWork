using System;
using ClearToWork.Domain.Entities.Equipment;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitAsset
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
}
