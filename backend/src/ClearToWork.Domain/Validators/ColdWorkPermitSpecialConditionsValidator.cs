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
    public class ColdWorkPermitSpecialConditionsValidator
    {
        public bool ValidateColdWork(bool eyeProtection, bool earDefenders)
        {
            return eyeProtection && earDefenders;
        }
    }
}