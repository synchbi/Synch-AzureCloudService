using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBDIntegrationWorker.SynchLibrary.Models
{
    public class SynchAccount
    {
        public int id { get; set; }

        public int businessId { get; set; }

        public string login { get; set; }

        public string password { get; set; }

        public int tier { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        public string sessionId { get; set; }

        public string deviceId { get; set; }
    }
}