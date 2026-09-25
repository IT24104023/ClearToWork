using System;

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
