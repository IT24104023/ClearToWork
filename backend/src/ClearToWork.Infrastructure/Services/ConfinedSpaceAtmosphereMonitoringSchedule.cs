using System;

namespace ClearToWork.Infrastructure.Services
{
    public class ConfinedSpaceAtmosphereMonitoringSchedule
    {
        public bool IsTestOverdue(DateTime lastTestTime) => (DateTime.UtcNow - lastTestTime).TotalMinutes > 30;
    }
}
