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
    public class CompetencyVerificationAuditLog
    {
        public void LogVerification(Guid workerId, string verifierId, bool approved)
        {
            Console.WriteLine($"[AUDIT] Worker {workerId} competency verified by {verifierId}: {approved}");
        }
    }
}
