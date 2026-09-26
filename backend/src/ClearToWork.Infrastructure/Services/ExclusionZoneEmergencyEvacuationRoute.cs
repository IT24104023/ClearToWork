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
    public class ExclusionZoneEmergencyEvacuationRoute
    {
        public List<string> GetEvacuationWaypoints(string currentZone)
        {
            return new List<string> { currentZone, "Muster Station 3 (Helideck South)" };
        }
    }
}