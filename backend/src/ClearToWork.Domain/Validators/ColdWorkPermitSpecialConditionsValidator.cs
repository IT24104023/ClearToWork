using System;

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
