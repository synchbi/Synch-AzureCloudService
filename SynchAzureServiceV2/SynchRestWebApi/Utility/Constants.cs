using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Utility
{
    public class Constants
    {
        public const string ERP_QBD_QUEUE = "erp-qbd-production";

        public class RequestHeaderKeys
        {
            public const string DEVICE_TYPE = "deviceType";
            public const string SESSION_ID = "sessionId";
            public const string ACCOUNT_ID = "accountId";
            public const string BUSINESS_ID = "businessId";
        }
    }
}