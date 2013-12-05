using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBDIntegrationWorker.SynchLibrary.Models
{
    public class SynchBusiness
    {

        public int id { get; set; }

        public string name { get; set; }

        public int integration { get; set; }

        public int tier { get; set; }

        public string address { get; set; }

        public string postalCode { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }
    }
}