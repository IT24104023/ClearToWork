using Xunit;
using System;
using ClearToWork.Domain.Entities.Permits;

namespace ClearToWork.Tests.Permits
{
    public class PermitHandoverTests
    {
        [Fact]
        public void HandoverLog_DefaultsToSiteInspectedTrue()
        {
            var log = new PermitHandoverShiftLog();
            Assert.True(log.SiteInspectedTogether);
        }
    }
}
