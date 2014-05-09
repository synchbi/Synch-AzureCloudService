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
    public class ERPAccountingClassEntity : TableEntity
    {
        public ERPAccountingClassEntity(int bid, string erpId)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = erpId;
        }

        public ERPAccountingClassEntity() { }

        public string className { get; set; }

    }
}
