using System;

namespace ClearToWork.Infrastructure.Services
{
    public class SCBACylinderPressureChecker
    {
        public bool VerifyPressure(double barReading, out string warning)
        {
            warning = string.Empty;
            if (barReading < 270.0)
            {
                warning = $"SCBA Cylinder pressure {barReading} bar is below 270 bar minimum duty threshold.";
                return false;
            }
            return true;
        }
    }
}
