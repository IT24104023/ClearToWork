using System;

namespace ClearToWork.Infrastructure.Services
{
    public class PermitRevalidationWorkflow
    {
        public bool CanExtend(int existingExtensions) => existingExtensions < 3;
    }
}
