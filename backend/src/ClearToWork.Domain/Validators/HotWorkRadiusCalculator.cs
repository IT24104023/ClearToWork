using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

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