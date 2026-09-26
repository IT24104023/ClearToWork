using System;

namespace ClearToWork.Domain.Entities.Permits
{
    public class WorkBreakdownStructureCostCodeLinker
    {
        public string GetWbsCode(string area) => $"WBS-2026-{area.ToUpper()}";
    }
}
