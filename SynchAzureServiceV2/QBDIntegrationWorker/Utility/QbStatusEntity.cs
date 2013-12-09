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
    public class QbStatusEntity : TableEntity
    {
        public QbStatusEntity(int bid, string dataSourcetype)
        {
            this.PartitionKey = dataSourcetype;
            this.RowKey = bid.ToString();
        }

        public QbStatusEntity() { }

        public DateTime integrationStartTime { get; set; }
        public DateTime integrationFinishTime { get; set; }

        public SyncStatusCode overallSyncStatusCode { get; set; }
        public SyncStatusCode invoiceSyncFromQbStatusCode { get; set; }
        public SyncStatusCode invoiceSyncFromSynchStatusCode { get; set; }
        public SyncStatusCode salesOrderSyncFromQbStatusCode { get; set; }
        public SyncStatusCode salesOrderSyncFromSynchStatusCode { get; set; }
        public SyncStatusCode customerSyncFromQbStatusCode { get; set; }
        public SyncStatusCode customerSyncFromSynchStatusCode { get; set; }
        public SyncStatusCode productSyncFromQbStatusCode { get; set; }
        public SyncStatusCode productSyncFromSynchStatusCode { get; set; }
        public SyncStatusCode salesRepSyncFromQbStatusCode { get; set; }
        public SyncStatusCode salesRepSyncFromSynchStatusCode { get; set; }

        public string syncResultLog { get; set; }


    }
}
