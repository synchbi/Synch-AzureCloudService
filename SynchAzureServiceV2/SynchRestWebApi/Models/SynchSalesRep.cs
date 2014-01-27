using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

namespace SynchRestWebApi.Models
{
    public class SynchSalesRep : TableEntity
    {
        public SynchSalesRep(int bid, string erpUniqueSalesRepId)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = erpUniqueSalesRepId;
        }

        public SynchSalesRep() { }

        public int accountIdFromSynch { get; set; }

        public string nameFromQb { get; set; }

        public string initialFromQb { get; set; }


    }
}