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
    public class SynchAccountingClass : TableEntity
    {
        public SynchAccountingClass(int businessId)
        {
            this.PartitionKey = businessId.ToString();
            this.RowKey = businessId.ToString();
        }

        public SynchAccountingClass() { }

        public string className { get; set; }
    }
}