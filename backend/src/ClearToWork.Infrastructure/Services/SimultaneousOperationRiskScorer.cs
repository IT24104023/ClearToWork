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
    /// <summary>
    /// Represents the calculated SIMOPS risk assessment score and hazard category.
    /// </summary>
    public class SimopsRiskScore
    {
        /// <summary>
        /// Numerical risk rating from 1 (lowest risk) to 10 (prohibited/extreme risk).
        /// </summary>
        public int Score { get; set; } = 1;

        /// <summary>
        /// Categorization of the SIMOPS risk level (e.g. LOW, MEDIUM, HIGH, CRITICAL_PROHIBITED).
        /// </summary>
        public string HazardCategory { get; set; } = "MEDIUM";
    }

    /// <summary>
    /// Contract for the simultaneous operation (SIMOPS) risk scoring service.
    /// </summary>
    public interface ISimultaneousOperationRiskScorer
    {
        /// <summary>
        /// Evaluates potential simultaneous activity conflicts between primary and secondary work activities.
        /// </summary>
        /// <param name="primaryActivity">Description of primary activity (e.g., Hot Work Welding).</param>
        /// <param name="secondaryActivity">Description of adjacent/secondary activity (e.g., Fuel Bunkering).</param>
        /// <returns>A calculated <see cref="SimopsRiskScore"/> object containing numerical score and risk classification.</returns>
        SimopsRiskScore EvaluateConflict(string primaryActivity, string secondaryActivity);
    }

    /// <summary>
    /// Service that analyzes concurrent industrial activities in the same or adjacent work zones
    /// and computes risk interaction scores based on SIMOPS conflict matrix standards.
    /// </summary>
    public class SimultaneousOperationRiskScorer : ISimultaneousOperationRiskScorer
    {
        /// <summary>
        /// Evaluates potential simultaneous activity conflicts between primary and secondary work activities.
        /// </summary>
        /// <param name="primaryActivity">Description of primary activity (e.g., Hot Work Welding).</param>
        /// <param name="secondaryActivity">Description of adjacent/secondary activity (e.g., Fuel Bunkering).</param>
        /// <returns>A calculated <see cref="SimopsRiskScore"/> object containing numerical score and risk classification.</returns>
        public SimopsRiskScore EvaluateConflict(string primaryActivity, string secondaryActivity)
        {
            var res = new SimopsRiskScore();

            if (string.IsNullOrWhiteSpace(primaryActivity) || string.IsNullOrWhiteSpace(secondaryActivity))
            {
                res.Score = 1;
                res.HazardCategory = "LOW";
                return res;
            }

            var p = primaryActivity.ToLowerInvariant();
            var s = secondaryActivity.ToLowerInvariant();

            if ((p.Contains("hot work") || p.Contains("welding")) && (s.Contains("bunkering") || s.Contains("hydrocarbon") || s.Contains("fuel")))
            {
                res.Score = 9;
                res.HazardCategory = "CRITICAL_PROHIBITED";
            }
            else if ((p.Contains("radiography") || p.Contains("nondestructive")) && (s.Contains("entry") || s.Contains("confined")))
            {
                res.Score = 8;
                res.HazardCategory = "HIGH";
            }
            else if (p.Contains("heavy lift") && s.Contains("scaffolding"))
            {
                res.Score = 7;
                res.HazardCategory = "HIGH";
            }
            else
            {
                res.Score = 3;
                res.HazardCategory = "MEDIUM";
            }

            return res;
        }
    }
}