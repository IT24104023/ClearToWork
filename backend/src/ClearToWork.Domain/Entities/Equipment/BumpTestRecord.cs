using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Equipment
{
    public class BumpTestRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SerialNumber { get; set; } = string.Empty;
        public bool PassedBumpTest { get; set; } = true;
        public DateTime TestTimestamp { get; set; } = DateTime.UtcNow;
        public string GasBottleBatchNo { get; set; } = "BOTTLE-2026-X";
    }
}
