using Xunit;
using ClearToWork.Domain.Validators;

namespace ClearToWork.Tests.Hazard
{
    public class NoiseZoneTests
    {
        [Fact]
        public void HighNoise_RequiresEarDefenders()
        {
            var validator = new HighNoiseZoneSafetyRequirementValidator();
            Assert.True(validator.CheckEarProtectionRequired(92.5));
        }
    }
}
