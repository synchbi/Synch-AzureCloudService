using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

namespace QuickBooksIntegrationWorker.Utility
{
    public class QbCredentialEntity : TableEntity
    {
        public QbCredentialEntity(string businessName, int bid, string dataSourcetype)
        {
            this.PartitionKey = dataSourcetype;
            this.RowKey = bid.ToString();
        }

        public QbCredentialEntity() { }

        public string realmId { get; set; }

        public string accessToken { get; set; }

        public string accessTokenSecret { get; set; }

        public string consumerKey { get; set; }

        public string consumerSecret { get; set; }

        public string dataSourcetype { get; set; }
    }   
}
