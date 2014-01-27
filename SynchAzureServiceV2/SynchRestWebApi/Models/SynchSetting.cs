﻿using System;
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
    public class SynchSetting : TableEntity
    {
        public SynchSetting(int businessId)
        {
            this.PartitionKey = businessId.ToString();
            this.RowKey = businessId.ToString();
        }

        public SynchSetting() { }

        public int businessId { get; set; }

        public bool showHistoryTab { get; set; }

        public bool showSalesStatistics { get; set; }

        public bool showQuantityOnPurchaseOrder { get; set; }

        public bool allowNegativeInventory { get; set; }

        public bool allowOfflineMode { get; set; }

        // ERP Integration settings
        // from IntegrationConfiguration.cs
        public string timezone { get; set; }

        public DateTime historyStartDate { get; set; }

        public int defaultAccountId { get; set; }

        public bool syncOrderAsInvoice { get; set; }

        public bool isInitialSync { get; set; }

        public bool resyncHistory { get; set; }

        public bool resyncInventory { get; set; }

        public bool resyncCustomer { get; set; }
    }
}