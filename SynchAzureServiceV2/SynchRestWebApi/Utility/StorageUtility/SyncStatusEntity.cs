using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

namespace SynchRestWebApi.Utility.StorageUtility
{
    public class SyncStatusEntity : TableEntity
    {
        public SyncStatusEntity(int bid, string dataSourcetype)
        {
            this.PartitionKey = dataSourcetype;
            this.RowKey = bid.ToString();
        }

        public SyncStatusEntity() { }

        public DateTime integrationStartTime { get; set; }
        public DateTime integrationFinishTime { get; set; }

        public int overallSyncStatusCode { get; set; }
        public int invoiceSyncFromQbStatusCode { get; set; }
        public int invoiceSyncFromSynchStatusCode { get; set; }
        public int salesOrderSyncFromQbStatusCode { get; set; }
        public int salesOrderSyncFromSynchStatusCode { get; set; }
        public int customerSyncFromQbStatusCode { get; set; }
        public int customerSyncFromSynchStatusCode { get; set; }
        public int productSyncFromQbStatusCode { get; set; }
        public int productSyncFromSynchStatusCode { get; set; }
        public int salesRepSyncFromQbStatusCode { get; set; }
        public int salesRepSyncFromSynchStatusCode { get; set; }

        public string syncResultLog { get; set; }


    }
}