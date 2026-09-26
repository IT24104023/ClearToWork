using Xunit;
using ClearToWork.Infrastructure.Services;

namespace ClearToWork.Tests.Permits
{
    public class PermitExtensionTests
    {
        [Fact]
        public void Max3Extensions_Enforced()
        {
            var workflow = new PermitRevalidationWorkflow();
            Assert.False(workflow.CanExtend(3));
        }
    }
}
