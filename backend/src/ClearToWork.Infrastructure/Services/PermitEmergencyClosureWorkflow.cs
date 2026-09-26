using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Infrastructure.Services
{
    public class PermitEmergencyClosureWorkflow
    {
        public void TriggerEmergencyShutdown(Guid permitId, string reason)
        {
            Console.WriteLine($"[STOP WORK AUTHORIZATION] Permit {permitId} IMMEDIATELY SUSPENDED: {reason}");
        }
    }
}