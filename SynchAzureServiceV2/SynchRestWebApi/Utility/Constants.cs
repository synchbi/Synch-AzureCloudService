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
            public const string DEVICE_TYPE = "SYNCH-DEVICETYPE";
            public const string SESSION_ID = "SYNCH-SESSIONID";
            public const string ACCOUNT_ID = "SYNCH-ACCOUNTID";
            public const string BUSINESS_ID = "SYNCH-BUSINESSID";
            public const string PASSWORD = "SYNCH-PASSWORD";
        }
    }
}