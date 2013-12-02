using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace SynchRestWebApi.Models
{
    public class SynchError
    {
        public class SynchErrorCode
        {
            public const int ACTION_POST = 0;
            public const int ACTION_GET = 1;
            public const int ACTION_PUT = 2;
            public const int ACTION_DELETE = 3;

            public const int SERVICE_ACCOUNT = 0;
            public const int SERVICE_BUSINESS = 1;
            public const int SERVICE_CUSTOMER = 2;
            public const int SERVICE_SUPPLIER = 3;
            public const int SERVICE_INVENTORY = 4;
            public const int SERVICE_RECORD = 5;

            // account manager errors

            // business manager errors

            // customer manager errors

            // supplier manager errors

            // inventory manager errors

            // record manager errors

        }

        public string errorCode { get; set; }
        public string errorMessage { get; set; }

        public SynchError(HttpRequestMessage request, int action, int service, string message)
        {
            // gets Seattle time so that I do not need to convert the timezone in my head!!!!!
            DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            if (request != null)
            {
                System.Diagnostics.Trace.TraceWarning(request.ToString());
                System.Diagnostics.Trace.TraceWarning(currentDateTimePST.ToString() + ": " + message);
            }

            errorCode = action.ToString() + ":" + service.ToString();
            errorMessage = currentDateTimePST.ToString() + ": " + message;
        }
    }
}