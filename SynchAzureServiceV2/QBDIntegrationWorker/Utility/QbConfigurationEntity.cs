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

    public class QbConfigurationEntity : TableEntity
    {
        public QbConfigurationEntity(int bid, string dataSourcetype)
        {
            this.PartitionKey = dataSourcetype;
            this.RowKey = bid.ToString();
        }

        public QbConfigurationEntity() { }

        // false if this business never synced before
        public bool initialized { get; set; }

        public bool resyncBusiness { get; set; }

        public bool resyncItem { get; set; }

        public bool resyncHistory { get; set; }

        public bool syncOrderAsInvoice { get; set; }

        public string timezone { get; set; }
    }
}
