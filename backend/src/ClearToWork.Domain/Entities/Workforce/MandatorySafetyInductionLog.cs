using System;

namespace ClearToWork.Domain.Entities.Workforce
{
    public class MandatorySafetyInductionLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkerId { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        public string InstallationName { get; set; } = "Alpha Platform Complex";
    }
}
