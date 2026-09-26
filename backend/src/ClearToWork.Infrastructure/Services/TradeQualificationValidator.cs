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
    public class TradeQualificationResult
    {
        public bool IsQualified { get; set; }
        public List<string> MissingQualifications { get; set; } = new();
    }

    public class TradeQualificationValidator
    {
        public TradeQualificationResult Validate(string trade, List<string> activeCertificates)
        {
            var res = new TradeQualificationResult { IsQualified = true };
            if (trade.Equals("Rig Electrician", StringComparison.OrdinalIgnoreCase) && !activeCertificates.Contains("COMPEX_EX01"))
            {
                res.IsQualified = false;
                res.MissingQualifications.Add("CompEx EX01 Explosive Atmosphere Electrical Certification");
            }
            return res;
        }
    }
}