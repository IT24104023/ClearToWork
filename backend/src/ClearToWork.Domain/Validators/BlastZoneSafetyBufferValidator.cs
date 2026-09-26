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
    public class BlastZoneSafetyBufferValidator
    {
        public bool IsPermitSafeFromPressurizedLine(double distanceMeters, double linePressureBar)
        {
            double requiredBuffer = linePressureBar * 0.5;
            return distanceMeters >= requiredBuffer;
        }
    }
}
