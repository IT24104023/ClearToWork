using Xunit;
using ClearToWork.Infrastructure.Services;

namespace ClearToWork.Tests.Workforce
{
    public class EmergencyTeamTests
    {
        [Fact]
        public void Coxswain_AssignsLifeboatRole()
        {
            var assigner = new EmergencyResponseTeamRoleAssigner();
            var role = assigner.AssignRole("ADVANCED_COXSWAIN_OPITO");
            Assert.Equal("Lifeboat Coxswain", role);
        }
    }
}
