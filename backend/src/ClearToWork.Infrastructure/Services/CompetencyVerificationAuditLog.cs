using System;

namespace ClearToWork.Infrastructure.Services
{
    public class CompetencyVerificationAuditLog
    {
        public void LogVerification(Guid workerId, string verifierId, bool approved)
        {
            Console.WriteLine($"[AUDIT] Worker {workerId} competency verified by {verifierId}: {approved}");
        }
    }
}
