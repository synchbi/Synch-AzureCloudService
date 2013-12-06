using System;
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
    public class ERPAccountMapEntity : TableEntity
    {
        public ERPAccountMapEntity(int bid, string erpUniqueSalesRepId)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = erpUniqueSalesRepId;
        }

        public ERPAccountMapEntity() { }

        public int accountIdFromSynch { get; set; }

    }
}
