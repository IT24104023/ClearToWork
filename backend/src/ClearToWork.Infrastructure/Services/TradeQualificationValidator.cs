using System;
using System.Collections.Generic;

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
