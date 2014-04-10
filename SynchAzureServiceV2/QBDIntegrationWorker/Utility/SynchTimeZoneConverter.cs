using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBDIntegrationWorker.Utility
{
    public class SynchTimeZoneConverter
    {

        public static double getLocalToUtcHourDifference(DateTime utcDateTime, string localTimeZone)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                utcDateTime = utcDateTime.ToUniversalTime();

            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById(localTimeZone));


            return (localDateTime - utcDateTime).TotalHours;
        }

        public static double getUtcToLocalHourDifference(DateTime utcDateTime, string localTimeZone)
        {
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById(localTimeZone));


            return (utcDateTime - localDateTime).TotalHours;
        }
    }
}
