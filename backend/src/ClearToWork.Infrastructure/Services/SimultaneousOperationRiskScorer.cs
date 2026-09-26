using System;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Infrastructure.Services
{
    public class SimopsRiskScore
    {
        public int Score { get; set; } // 1-10
        public string HazardCategory { get; set; } = "MEDIUM";
    }

    public class SimultaneousOperationRiskScorer
    {
        public SimopsRiskScore EvaluateConflict(string primaryActivity, string secondaryActivity)
        {
            var res = new SimopsRiskScore();
            if (primaryActivity.Contains("Hot Work") && secondaryActivity.Contains("Bunkering"))
            {
                res.Score = 9;
                res.HazardCategory = "CRITICAL_PROHIBITED";
            }
            return res;
        }
    }
}