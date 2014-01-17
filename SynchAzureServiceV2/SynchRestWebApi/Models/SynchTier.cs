using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

namespace SynchRestWebApi.Models
{
    public class SynchTier : TableEntity
    {
        public SynchTier(int businessId, int tierId)
        {
            this.PartitionKey = businessId.ToString();
            this.RowKey = tierId.ToString();
        }

        public SynchTier() { }

        // tier ID
        public int id { get; set; }
        public int businessId { get; set; }

        public string tierName { get; set; }

        // access to actions
        public bool allowRecordDiscount { get; set; }
        public bool allowLineItemNote { get; set; }
        public bool allowPresentRecord { get; set; }
        public bool allowRecordComment { get; set; }
        public bool allowInventoryAdjustment { get; set; }

        public bool allowCreatingCustomer { get; set; }
        public bool allowCreatingSupplier { get; set; }
        public bool allowCreatingProduct { get; set; }

        public bool allowUpdatingCustomer { get; set; }
        public bool allowUpdatingSupplier { get; set; }
        public bool allowUpdatingProduct { get; set; }

        // access to data and view
        public bool showCreateOrderTab { get; set; }
        public bool showCustomerActivityTab { get; set; }
        public bool showInventoryTab { get; set; }
        public bool showReceiveProductTab { get; set; }

        public bool showOverallCustomer { get; set; }
        public bool showOverallSupplier { get; set; }
        public bool showOverallRecords { get; set; }

    }
}