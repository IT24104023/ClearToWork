using System;

namespace ClearToWork.Infrastructure.Services
{
    /// <summary>
    /// Contract for confined space atmospheric testing schedule monitor.
    /// </summary>
    public interface IConfinedSpaceAtmosphereMonitoringSchedule
    {
        /// <summary>
        /// Evaluates whether the mandatory 30-minute periodic gas re-test is overdue for a confined space permit.
        /// </summary>
        /// <param name="lastTestTime">Timestamp of last recorded gas test.</param>
        /// <returns>True if test is overdue (>30 mins); otherwise false.</returns>
        bool IsTestOverdue(DateTime lastTestTime);
    }

    /// <summary>
    /// Service that enforces OSHA 1910.146 continuous atmospheric monitoring interval rules for confined space entry operations.
    /// </summary>
    public class ConfinedSpaceAtmosphereMonitoringSchedule : IConfinedSpaceAtmosphereMonitoringSchedule
    {
        /// <summary>
        /// Maximum allowed interval between mandatory atmospheric tests in minutes.
        /// </summary>
        public const int MaxTestIntervalMinutes = 30;

        /// <summary>
        /// Evaluates whether the mandatory 30-minute periodic gas re-test is overdue for a confined space permit.
        /// </summary>
        /// <param name="lastTestTime">Timestamp of last recorded gas test.</param>
        /// <returns>True if test is overdue (>30 mins); otherwise false.</returns>
        public bool IsTestOverdue(DateTime lastTestTime)
        {
            if (lastTestTime == default) return true;
            return (DateTime.UtcNow - lastTestTime).TotalMinutes > MaxTestIntervalMinutes;
        }
    }
}
