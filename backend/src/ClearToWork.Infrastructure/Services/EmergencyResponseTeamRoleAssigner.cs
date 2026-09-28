using System;

namespace ClearToWork.Infrastructure.Services
{
    /// <summary>
    /// Contract for Emergency Response Team (ERT) role assignment service.
    /// </summary>
    public interface IEmergencyResponseTeamRoleAssigner
    {
        /// <summary>
        /// Assigns an offshore Emergency Response Team role based on worker certification.
        /// </summary>
        /// <param name="qualification">Certification code or title (e.g. COXSWAIN, FIRE_FIGHTING).</param>
        /// <returns>Assigned ERT role title.</returns>
        string AssignRole(string qualification);
    }

    /// <summary>
    /// Service that maps worker certifications to active Emergency Response Team (ERT) duties on offshore installations.
    /// </summary>
    public class EmergencyResponseTeamRoleAssigner : IEmergencyResponseTeamRoleAssigner
    {
        /// <summary>
        /// Assigns an offshore Emergency Response Team role based on worker certification.
        /// </summary>
        /// <param name="qualification">Certification code or title (e.g. COXSWAIN, FIRE_FIGHTING).</param>
        /// <returns>Assigned ERT role title.</returns>
        public string AssignRole(string qualification)
        {
            if (string.IsNullOrWhiteSpace(qualification)) return "General Muster Member";

            var q = qualification.ToUpperInvariant();
            if (q.Contains("COXSWAIN")) return "Lifeboat Coxswain";
            if (q.Contains("FIRE_FIGHTING") || q.Contains("FIREFIGHTER")) return "Helideck Firefighter";
            if (q.Contains("FIRST_AID") || q.Contains("PARAMEDIC")) return "First Aid Responder";

            return "General Muster Member";
        }
    }
}
