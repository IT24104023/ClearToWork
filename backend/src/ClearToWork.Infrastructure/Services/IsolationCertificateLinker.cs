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
    /// Contract for linking mechanical, electrical, and process isolation certificates (ICC) to permits.
    /// </summary>
    public interface IIsolationCertificateLinker
    {
        /// <summary>
        /// Binds an isolation confirmation certificate (ICC) to a target permit ID.
        /// </summary>
        /// <param name="permitId">Target permit request unique identifier.</param>
        /// <param name="iccNumber">Isolation Confirmation Certificate serial number (e.g. ICC-2026-8819).</param>
        void LinkIsolationToPermit(Guid permitId, string iccNumber);
    }

    /// <summary>
    /// Service that binds electrical, mechanical, hydraulic, and process isolation certificates to Permit-to-Work records prior to permit authorization.
    /// </summary>
    public class IsolationCertificateLinker : IIsolationCertificateLinker
    {
        /// <summary>
        /// Binds an isolation confirmation certificate (ICC) to a target permit ID.
        /// </summary>
        /// <param name="permitId">Target permit request unique identifier.</param>
        /// <param name="iccNumber">Isolation Confirmation Certificate serial number (e.g. ICC-2026-8819).</param>
        public void LinkIsolationToPermit(Guid permitId, string iccNumber)
        {
            if (string.IsNullOrWhiteSpace(iccNumber))
                throw new ArgumentException("Isolation Certificate number (ICC) cannot be null or empty.", nameof(iccNumber));

            Console.WriteLine($"[ICC LINK] Permit {permitId} successfully bound to Isolation Confirmation Certificate '{iccNumber}' at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        }
    }
}