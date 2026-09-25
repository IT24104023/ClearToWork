using System;

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
