using Xunit;
using ClearToWork.Infrastructure.Services;

namespace ClearToWork.Tests.Hazard
{
    public class SimopsRiskScorerTests
    {
        [Fact]
        public void HotWork_And_Bunkering_Returns_Critical()
        {
            var scorer = new SimultaneousOperationRiskScorer();
            var res = scorer.EvaluateConflict("Hot Work Grinding", "Diesel Bunkering");
            Assert.Equal("CRITICAL_PROHIBITED", res.HazardCategory);
        }
    }
}
