using System;

namespace ClearToWork.Domain.Entities.Permits
{
    public class PermitHandoverShiftLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PermitId { get; set; }
        public string OutgoingIssuingAuthority { get; set; } = string.Empty;
        public string IncomingIssuingAuthority { get; set; } = string.Empty;
        public DateTime HandoverTime { get; set; } = DateTime.UtcNow;
        public bool SiteInspectedTogether { get; set; } = true;
    }
}
