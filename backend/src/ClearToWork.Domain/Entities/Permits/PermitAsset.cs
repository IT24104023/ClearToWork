using System;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitAsset
{
    public Guid PermitRequestId { get; set; }
    public PermitRequest? PermitRequest { get; set; }
    public Guid AssetId { get; set; }
    public Asset? Asset { get; set; }
    public DateTime? ReturnedAt { get; set; }
}