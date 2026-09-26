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
    public class IsolationCertificateLinker
    {
        public void LinkIsolationToPermit(Guid permitId, string iccNumber)
        {
            Console.WriteLine($"[ICC LINK] Permit {permitId} bound to Isolation Certificate {iccNumber}");
        }
    }
}