using System;
using System.Threading.Tasks;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Infrastructure.Services
{
    public class FatigueStatus
    {
        public bool CanWorkShift { get; set; } = true;
        public double HoursWorkedLast24h { get; set; }
        public string Recommendation { get; set; } = "Approved for 12-hour duty shift";
    }

    public class FatigueManagementTracker
    {
        public Task<FatigueStatus> CheckFatigueAsync(Guid workerId, double upcomingShiftHours)
        {
            var status = new FatigueStatus { HoursWorkedLast24h = 10.5 };
            if (status.HoursWorkedLast24h + upcomingShiftHours > 14)
            {
                status.CanWorkShift = false;
                status.Recommendation = "Mandatory 8-hour rest period required under OGUK Fatigue Guidelines.";
            }
            return Task.FromResult(status);
        }
    }
}