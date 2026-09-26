using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

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