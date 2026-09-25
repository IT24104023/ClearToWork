using System;

namespace ClearToWork.Infrastructure.Services
{
    public class EmergencyResponseTeamRoleAssigner
    {
        public string AssignRole(string qualification)
        {
            if (qualification.Contains("COXSWAIN")) return "Lifeboat Coxswain";
            if (qualification.Contains("FIRE_FIGHTING")) return "Helideck Firefighter";
            return "General Muster Member";
        }
    }
}
