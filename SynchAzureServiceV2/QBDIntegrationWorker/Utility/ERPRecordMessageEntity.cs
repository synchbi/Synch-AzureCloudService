﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

namespace QBDIntegrationWorker.Utility
{
    public class ERPRecordMessageEntity : TableEntity
    {
        public ERPRecordMessageEntity(int bid, int recordId)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = recordId.ToString();
        }

        public ERPRecordMessageEntity() { }

        public int action { get; set; }

        public bool active { get; set; }

        public int status { get; set; }

        public string log { get; set; }

    }
}