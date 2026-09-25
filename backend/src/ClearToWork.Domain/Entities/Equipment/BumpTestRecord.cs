using System;

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
