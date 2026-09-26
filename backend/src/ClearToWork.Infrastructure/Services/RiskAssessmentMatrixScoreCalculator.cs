using System;

namespace ClearToWork.Infrastructure.Services
{
    public class RiskAssessmentMatrixScoreCalculator
    {
        public int CalculateMatrixRisk(int likelihood, int severity) => likelihood * severity;
    }
}
