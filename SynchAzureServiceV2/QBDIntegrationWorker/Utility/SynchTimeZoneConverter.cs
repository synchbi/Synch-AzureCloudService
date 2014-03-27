using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBDIntegrationWorker.Utility
{
    public class SynchTimeZoneConverter
    {

        public static int getLocalToUtcHourDifference(string localTimeZone)
        {
            switch (localTimeZone)
            {
                case "Pacific Standard Time":
                    return -8;
                case "Pacific Daylight Time":
                    return -7;
                case "Eastern Standard Time":
                    return -5;
                case "Eastern Daylight Time":
                    return -4;
                default:
                    return 0;
            }
        }
    }
}
