using System;
using System.Collections.Generic;

namespace ClearToWork.Infrastructure.Services
{
    public class EquipmentMaintenanceHistoryLog
    {
        public List<string> GetLogs(string serialNumber)
        {
            return new List<string> { $"2026-09-20: Sensor diaphragm replacement [{serialNumber}]" };
        }
    }
}
