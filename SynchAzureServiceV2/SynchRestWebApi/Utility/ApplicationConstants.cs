using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Utility
{
    public class ApplicationConstants
    {
        //public static string ERP_QBD_QUEUE = "erp-qbd-production";
        //public const string ERP_QBD_QUEUE = "erp-qbd";
        public static string ERP_QBD_QUEUE = "erp-qbd-test";

        public const string ERP_QBD_TABLE_INFO = "quickbooksinfo";
        public const string ERP_QBD_TABLE_CONFIG = "quickbooksconfig";
        public const string ERP_QBD_TABLE_STATUS = "quickbooksstatus";

        public const string ERP_QBD_TABLE_PRODUCT = "erpproductmapping";
        public const string ERP_QBD_TABLE_BUSINESS = "erpbusinessmapping";
        public const string ERP_QBD_TABLE_RECORD = "erprecordmapping";
        public const string ERP_QBD_TABLE_ACCOUNT = "erpaccountmapping";

        public const string DEFAULT_TIME_ZONE = "Pacific Standard Time";

        public const string DASHBOARD_LINK = "synchbi.com/dashboard";
    }

}