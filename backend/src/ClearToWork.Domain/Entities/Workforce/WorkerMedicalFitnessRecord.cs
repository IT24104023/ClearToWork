using System;

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
