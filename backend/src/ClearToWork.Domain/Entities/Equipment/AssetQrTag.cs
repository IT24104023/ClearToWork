using System;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Equipment;

public class AssetQrTag : BaseEntity
{
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
    public string QrCodeValue { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}
