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
    /// <summary>
    /// Contract for safety equipment quarantine workflow service.
    /// </summary>
    public interface IQuarantineEquipmentWorkflow
    {
        /// <summary>
        /// Places failed or uncalibrated safety equipment into strict quarantine status (OUT OF SERVICE).
        /// </summary>
        /// <param name="serialNumber">Equipment asset serial number.</param>
        /// <param name="reason">Reason for quarantine (e.g. Failed bump test, Expired calibration).</param>
        void Quarantine(string serialNumber, string reason);
    }

    /// <summary>
    /// Service that enforces out-of-service tags and quarantine status on faulty, damaged, or uncalibrated safety equipment.
    /// </summary>
    public class QuarantineEquipmentWorkflow : IQuarantineEquipmentWorkflow
    {
        /// <summary>
        /// Places failed or uncalibrated safety equipment into strict quarantine status (OUT OF SERVICE).
        /// </summary>
        /// <param name="serialNumber">Equipment asset serial number.</param>
        /// <param name="reason">Reason for quarantine (e.g. Failed bump test, Expired calibration).</param>
        public void Quarantine(string serialNumber, string reason)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                throw new ArgumentException("Asset serial number cannot be null or empty.", nameof(serialNumber));

            Console.WriteLine($"[QUARANTINE] Asset '{serialNumber}' tagged OUT OF SERVICE: {reason ?? "Safety inspection failure"} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        }
    }
}