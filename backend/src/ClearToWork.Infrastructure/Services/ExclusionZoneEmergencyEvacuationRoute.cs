using System.Collections.Generic;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Infrastructure.Services
{
    /// <summary>
    /// Contract for emergency evacuation route planner for exclusion zones.
    /// </summary>
    public interface IExclusionZoneEmergencyEvacuationRoute
    {
        /// <summary>
        /// Retrieves ordered emergency evacuation waypoints from specified zone to primary muster station.
        /// </summary>
        /// <param name="currentZone">Zone code or name (e.g. Zone A1).</param>
        /// <returns>List of evacuation waypoint descriptions.</returns>
        List<string> GetEvacuationWaypoints(string currentZone);
    }

    /// <summary>
    /// Service that maps primary and secondary emergency evacuation routes from high-risk exclusion zones to designated muster stations.
    /// </summary>
    public class ExclusionZoneEmergencyEvacuationRoute : IExclusionZoneEmergencyEvacuationRoute
    {
        /// <summary>
        /// Retrieves ordered emergency evacuation waypoints from specified zone to primary muster station.
        /// </summary>
        /// <param name="currentZone">Zone code or name (e.g. Zone A1).</param>
        /// <returns>List of evacuation waypoint descriptions.</returns>
        public List<string> GetEvacuationWaypoints(string currentZone)
        {
            var route = new List<string> { currentZone ?? "Unknown Zone" };

            if (currentZone?.ToUpperInvariant().Contains("A1") == true)
            {
                route.Add("Decontamination Portal A");
                route.Add("Muster Station 1 (Helideck South)");
            }
            else
            {
                route.Add("Primary Emergency Exit Stairwell B");
                route.Add("Muster Station 3 (Helideck South)");
            }

            return route;
        }
    }
}