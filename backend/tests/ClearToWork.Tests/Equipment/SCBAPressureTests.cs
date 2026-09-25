using Xunit;
using ClearToWork.Infrastructure.Services;

namespace ClearToWork.Tests.Equipment
{
    public class SCBAPressureTests
    {
        [Fact]
        public void LowPressure_FailsVerification()
        {
            var checker = new SCBACylinderPressureChecker();
            bool ok = checker.VerifyPressure(240.0, out var warn);
            Assert.False(ok);
            Assert.Contains("below 270 bar", warn);
        }
    }
}
