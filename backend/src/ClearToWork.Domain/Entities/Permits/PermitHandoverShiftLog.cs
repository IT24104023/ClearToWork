using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

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