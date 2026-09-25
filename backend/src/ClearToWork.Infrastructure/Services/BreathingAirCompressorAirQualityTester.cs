using System;

namespace ClearToWork.Infrastructure.Services
{
    public class BreathingAirCompressorAirQualityTester
    {
        public bool IsAirPure(double oilPpm, double moisturePpm) => oilPpm < 0.5 && moisturePpm < 50.0;
    }
}
