﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBDIntegrationWorker.SynchLibrary.Models
{
    public class SynchRecordLine
    {
        public int recordId { get; set; }

        public string upc { get; set; }

        public int quantity { get; set; }

        public decimal price { get; set; }

        public string note { get; set; }
    }
}