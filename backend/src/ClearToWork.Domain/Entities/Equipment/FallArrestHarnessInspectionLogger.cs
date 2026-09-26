using System;

namespace ClearToWork.Domain.Entities.Equipment
{
    public class FallArrestHarnessInspectionLogger
    {
        public void LogInspection(string serialNo, bool webbingIntact, bool dRingIntact) =>
            Console.WriteLine($"[HARNESS INSPECTION] {serialNo}: Webbing={webbingIntact}, DRing={dRingIntact}");
    }
}
