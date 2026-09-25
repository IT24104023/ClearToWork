using System;

namespace ClearToWork.Infrastructure.Services
{
    public class OffshoreSurvivalRefresherTracker
    {
        public bool NeedsBOSIETRefresher(DateTime lastCourseDate)
        {
            return (DateTime.UtcNow - lastCourseDate).TotalDays > 1460; // 4 years
        }
    }
}
