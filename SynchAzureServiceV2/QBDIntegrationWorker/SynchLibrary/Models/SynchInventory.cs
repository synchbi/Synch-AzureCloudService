using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBDIntegrationWorker.SynchLibrary.Models
{
    public class SynchInventory
    {

        public int businessId { get; set; }

        public string upc { get; set; }

        public string name { get; set; }

        public decimal defaultPrice { get; set; }

        public decimal purchasePrice { get; set; }

        public string detail { get; set; }

        public int leadTime { get; set; }

        public int quantityAvailable { get; set; }

        public int quantityOnPurchaseOrder { get; set; }

        public int reorderQuantity { get; set; }

        public int reorderPoint { get; set; }

        public int category { get; set; }

        public string location { get; set; }

        public string integrationId { get; set; }

        public int status { get; set; }

        public SynchInventory()
        {
            defaultPrice = Decimal.MinValue;
            leadTime = Int32.MinValue;
            quantityAvailable = Int32.MinValue;
            reorderPoint = Int32.MinValue;
            reorderQuantity = Int32.MinValue;
            category = Int32.MinValue;
            status = Int32.MinValue;
        }
    }
}