using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Models
{
    public class SynchSupplier : ISynchClient
    {
        public int businessId { get; set; }

        public int supplierId { get; set; }

        public int accountId { get; set; }

        public string address { get; set; }

        public string email { get; set; }

        public string phoneNumber { get; set; }

        public Nullable<int> category { get; set; }

        // from Business table
        public string name { get; set; }

        public string postalCode { get; set; }

        public string integrationId { get; set; }

        public int status { get; set; }
    }
}