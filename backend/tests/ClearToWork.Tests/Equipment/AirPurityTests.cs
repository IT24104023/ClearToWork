using Xunit;
using ClearToWork.Infrastructure.Services;

namespace ClearToWork.Tests.Equipment
{
    public class AirPurityTests
    {
        [Fact]
        public void CleanAir_PassesQualityTest()
        {
            var tester = new BreathingAirCompressorAirQualityTester();
            Assert.True(tester.IsAirPure(0.1, 20.0));
        }
    }
}
