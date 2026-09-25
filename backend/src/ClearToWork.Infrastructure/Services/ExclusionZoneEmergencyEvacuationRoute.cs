using System.Collections.Generic;

namespace ClearToWork.Infrastructure.Services
{
    public class ExclusionZoneEmergencyEvacuationRoute
    {
        public List<string> GetEvacuationWaypoints(string currentZone)
        {
            return new List<string> { currentZone, "Muster Station 3 (Helideck South)" };
        }
    }
}
