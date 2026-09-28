using System;
using System.Collections.Generic;
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
    /// Result object containing qualification outcome and any missing safety certifications.
    /// </summary>
    public class TradeQualificationResult
    {
        /// <summary>
        /// Indicates whether the worker meets all mandatory safety qualifications for their trade.
        /// </summary>
        public bool IsQualified { get; set; } = true;

        /// <summary>
        /// List of missing or expired certification titles required for compliance.
        /// </summary>
        public List<string> MissingQualifications { get; set; } = new();
    }

    /// <summary>
    /// Contract for trade qualification validation service.
    /// </summary>
    public interface ITradeQualificationValidator
    {
        /// <summary>
        /// Validates worker active certifications against trade requirements.
        /// </summary>
        /// <param name="trade">Trade role title (e.g. Rig Electrician, Pipefitter).</param>
        /// <param name="activeCertificates">List of active certification codes held by worker.</param>
        /// <returns>A <see cref="TradeQualificationResult"/> indicating qualification status.</returns>
        TradeQualificationResult Validate(string trade, List<string> activeCertificates);
    }

    /// <summary>
    /// Service that validates worker trade credentials against mandatory offshore industry safety standards.
    /// </summary>
    public class TradeQualificationValidator : ITradeQualificationValidator
    {
        /// <summary>
        /// Validates worker active certifications against trade requirements.
        /// </summary>
        /// <param name="trade">Trade role title (e.g. Rig Electrician, Pipefitter).</param>
        /// <param name="activeCertificates">List of active certification codes held by worker.</param>
        /// <returns>A <see cref="TradeQualificationResult"/> indicating qualification status.</returns>
        public TradeQualificationResult Validate(string trade, List<string> activeCertificates)
        {
            var res = new TradeQualificationResult { IsQualified = true };
            if (string.IsNullOrWhiteSpace(trade)) return res;

            var certs = activeCertificates ?? new List<string>();

            if (trade.Equals("Rig Electrician", StringComparison.OrdinalIgnoreCase) && !certs.Contains("COMPEX_EX01"))
            {
                res.IsQualified = false;
                res.MissingQualifications.Add("CompEx EX01 Explosive Atmosphere Electrical Certification");
            }

            if (trade.Equals("Scaffolder", StringComparison.OrdinalIgnoreCase) && !certs.Contains("CISRS_ADV"))
            {
                res.IsQualified = false;
                res.MissingQualifications.Add("CISRS Advanced Scaffolder Card");
            }

            return res;
        }
    }
}