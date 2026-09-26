using System;

namespace ClearToWork.Domain.Validators
{
    public class HighNoiseZoneSafetyRequirementValidator
    {
        public bool CheckEarProtectionRequired(double noiseLevelDb) => noiseLevelDb > 85.0;
    }
}
