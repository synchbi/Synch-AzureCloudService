using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Models
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

        public System.DateTimeOffset transactionDate { get; set; }

        public Nullable<System.DateTimeOffset> deliveryDate { get; set; }

        public List<SynchRecordLine> recordLines { get; set; }
    }
}