using System;

namespace ClearToWork.Infrastructure.Services
{
    /// <summary>
    /// Contract for breathing air quality testing service.
    /// </summary>
    public interface IBreathingAirCompressorAirQualityTester
    {
        /// <summary>
        /// Validates compressed breathing air purity levels against EN 12021 standards.
        /// </summary>
        /// <param name="oilPpm">Oil mist concentration in mg/m3 or ppm.</param>
        /// <param name="moisturePpm">Water vapor concentration in ppm.</param>
        /// <returns>True if air purity meets safety standards; otherwise false.</returns>
        bool IsAirPure(double oilPpm, double moisturePpm);
    }

    /// <summary>
    /// Service that validates breathing air compressor quality output against EN 12021 and OSHA compressed air standards.
    /// </summary>
    public class BreathingAirCompressorAirQualityTester : IBreathingAirCompressorAirQualityTester
    {
        /// <summary>
        /// Maximum allowed oil mist concentration in ppm.
        /// </summary>
        public const double MaxOilPpm = 0.5;

        /// <summary>
        /// Maximum allowed moisture concentration in ppm.
        /// </summary>
        public const double MaxMoisturePpm = 50.0;

        /// <summary>
        /// Validates compressed breathing air purity levels against EN 12021 standards.
        /// </summary>
        /// <param name="oilPpm">Oil mist concentration in mg/m3 or ppm.</param>
        /// <param name="moisturePpm">Water vapor concentration in ppm.</param>
        /// <returns>True if air purity meets safety standards; otherwise false.</returns>
        public bool IsAirPure(double oilPpm, double moisturePpm)
        {
            return oilPpm < MaxOilPpm && moisturePpm < MaxMoisturePpm;
        }
    }
}
