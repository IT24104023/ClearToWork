using System;

namespace ClearToWork.Infrastructure.Services
{
    public class GasCloudDispersionModeler
    {
        public double EstimateDownwindPlumeMeters(double releaseRateKgSec, double windKnots)
        {
            return (releaseRateKgSec * 100.0) / Math.Max(windKnots, 1.0);
        }
    }
}
