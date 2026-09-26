using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Workforce
{
    public class WorkerMedicalFitnessRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkerId { get; set; }
        public bool OffshoreFit { get; set; } = true;
        public bool ConfinedSpaceClearance { get; set; } = true;
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(1);
        public string MedicalOfficerNotes { get; set; } = "Fit for full duty";
    }
}
