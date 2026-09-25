using System;

namespace ClearToWork.Infrastructure.Services
{
    public class QuarantineEquipmentWorkflow
    {
        public void Quarantine(string serialNumber, string reason)
        {
            Console.WriteLine($"[QUARANTINE] Asset {serialNumber} tagged OUT OF SERVICE: {reason}");
        }
    }
}
