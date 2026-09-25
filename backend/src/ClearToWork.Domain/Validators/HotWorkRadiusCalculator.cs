using System;

namespace ClearToWork.Domain.Validators
{
    public class HotWorkRadiusCalculator
    {
        public double CalculateSafetyZoneRadius(string hotWorkType, double windSpeedKts)
        {
            double baseRadius = hotWorkType.Equals("Grinding", StringComparison.OrdinalIgnoreCase) ? 15.0 : 10.0;
            if (windSpeedKts > 20) baseRadius += 5.0;
            return baseRadius;
        }
    }
}
