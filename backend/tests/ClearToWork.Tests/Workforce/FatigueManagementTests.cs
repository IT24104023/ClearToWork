using Xunit;
using System;
using System.Threading.Tasks;
using ClearToWork.Infrastructure.Services;

namespace ClearToWork.Tests.Workforce
{
    public class FatigueManagementTests
    {
        [Fact]
        public async Task ExcessiveHours_TriggersRestPeriodRecommendation()
        {
            var tracker = new FatigueManagementTracker();
            var res = await tracker.CheckFatigueAsync(Guid.NewGuid(), 5.0);
            Assert.False(res.CanWorkShift);
            Assert.Contains("Mandatory", res.Recommendation);
        }
    }
}
