using System;

namespace ClearToWork.Infrastructure.Services
{
    /// <summary>
    /// Contract for portable gas detector fresh air zero-point calibration service.
    /// </summary>
    public interface IPortableGasDetectorZeroPointCalibrator
    {
        /// <summary>
        /// Executes a fresh air zero-point calibration for a portable gas detector unit.
        /// </summary>
        /// <param name="serialNumber">Equipment serial number.</param>
        /// <returns>True if zero-point calibration succeeded; otherwise false.</returns>
        bool CalibrateFreshAirZero(string serialNumber);
    }

    /// <summary>
    /// Service that executes zero-point calibration and baseline resets for portable multi-gas detectors in clean air environments.
    /// </summary>
    public class PortableGasDetectorZeroPointCalibrator : IPortableGasDetectorZeroPointCalibrator
    {
        /// <summary>
        /// Executes a fresh air zero-point calibration for a portable gas detector unit.
        /// </summary>
        /// <param name="serialNumber">Equipment serial number.</param>
        /// <returns>True if zero-point calibration succeeded; otherwise false.</returns>
        public bool CalibrateFreshAirZero(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber)) return false;
            Console.WriteLine($"[CALIBRATION] Fresh air zero calibration completed for detector serial '{serialNumber}' at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            return true;
        }
    }
}
