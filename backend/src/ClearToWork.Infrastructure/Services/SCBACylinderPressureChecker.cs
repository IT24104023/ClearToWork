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
    /// Contract for SCBA breathing cylinder pressure verification service.
    /// </summary>
    public interface ISCBACylinderPressureChecker
    {
        /// <summary>
        /// Verifies SCBA cylinder pressure reading against 270 bar minimum duty threshold.
        /// </summary>
        /// <param name="barReading">Pressure reading in bars.</param>
        /// <param name="warning">Out variable containing safety warning if pressure is below threshold.</param>
        /// <returns>True if pressure is sufficient; otherwise false.</returns>
        bool VerifyPressure(double barReading, out string warning);
    }

    /// <summary>
    /// Service that checks Self-Contained Breathing Apparatus (SCBA) cylinder pressure before issue for toxic gas or oxygen-deficient entry.
    /// </summary>
    public class SCBACylinderPressureChecker : ISCBACylinderPressureChecker
    {
        /// <summary>
        /// Minimum operational cylinder pressure in bars.
        /// </summary>
        public const double MinimumOperationalBar = 270.0;

        /// <summary>
        /// Verifies SCBA cylinder pressure reading against 270 bar minimum duty threshold.
        /// </summary>
        /// <param name="barReading">Pressure reading in bars.</param>
        /// <param name="warning">Out variable containing safety warning if pressure is below threshold.</param>
        /// <returns>True if pressure is sufficient; otherwise false.</returns>
        public bool VerifyPressure(double barReading, out string warning)
        {
            warning = string.Empty;
            if (barReading < MinimumOperationalBar)
            {
                warning = $"SCBA Cylinder pressure {barReading:F1} bar is below the {MinimumOperationalBar} bar minimum duty threshold for hazardous area entry.";
                return false;
            }
            return true;
        }
    }
}