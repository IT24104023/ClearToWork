using System;

namespace ClearToWork.Domain.Validators
{
    public class BlastZoneSafetyBufferValidator
    {
        public bool IsPermitSafeFromPressurizedLine(double distanceMeters, double linePressureBar)
        {
            double requiredBuffer = linePressureBar * 0.5;
            return distanceMeters >= requiredBuffer;
        }
    }
}
