using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBooksIntegrationWorker.SynchLibrary.Models
{
    public class SynchRecord
    {
        public int id { get; set; }

        public int category { get; set; }

        public int accountId { get; set; }

        public int ownerId { get; set; }

        public int clientId { get; set; }

        public int status { get; set; }

        public string title { get; set; }

        public string comment { get; set; }

        /*
            By default, Json.NET writes dates in ISO 8601 format. Dates in UTC (Coordinated Universal Time) are written with a "Z" suffix. Dates in local time include a time-zone offset. For example:

            2012-07-27T18:51:45.53403Z         // UTC
            2012-07-27T11:51:45.53403-08:00    // Local (PST)
            2012-07-27T11:51:45.53403-07:00    // Local (PDT)

            By default, Json.NET preserves the time zone. You can override this by setting the DateTimeZoneHandling property:

            // Convert all dates to UTC
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        */
        public System.DateTimeOffset transactionDate { get; set; }

        public Nullable<System.DateTimeOffset> deliveryDate { get; set; }

        public List<SynchRecordLine> recordLines { get; set; }
    }
}