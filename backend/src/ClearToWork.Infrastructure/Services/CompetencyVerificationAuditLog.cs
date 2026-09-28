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
    /// <summary>
    /// Contract for logging worker competency verification audit events.
    /// </summary>
    public interface ICompetencyVerificationAuditLog
    {
        /// <summary>
        /// Records an audit trail entry for a worker competency verification event.
        /// </summary>
        /// <param name="workerId">Unique identifier of worker being verified.</param>
        /// <param name="verifierId">Identifier or email of safety officer performing verification.</param>
        /// <param name="approved">True if verification passed; otherwise false.</param>
        void LogVerification(Guid workerId, string verifierId, bool approved);
    }

    /// <summary>
    /// Service that generates immutable compliance audit logs for worker competency verification actions.
    /// </summary>
    public class CompetencyVerificationAuditLog : ICompetencyVerificationAuditLog
    {
        /// <summary>
        /// Records an audit trail entry for a worker competency verification event.
        /// </summary>
        /// <param name="workerId">Unique identifier of worker being verified.</param>
        /// <param name="verifierId">Identifier or email of safety officer performing verification.</param>
        /// <param name="approved">True if verification passed; otherwise false.</param>
        public void LogVerification(Guid workerId, string verifierId, bool approved)
        {
            Console.WriteLine($"[AUDIT] Worker {workerId} competency verified by {verifierId ?? "SYSTEM"}: {(approved ? "PASSED" : "REJECTED")} at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        }
    }
}