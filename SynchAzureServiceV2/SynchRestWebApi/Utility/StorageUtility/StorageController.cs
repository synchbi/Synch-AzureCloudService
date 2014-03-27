using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// for table storage
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace SynchRestWebApi.Utility.StorageUtility
{
    public class StorageController
    {

        public static CloudTable setupTable(string tableRefName)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            tableClient.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(1), 5);

            CloudTable table = tableClient.GetTableReference(tableRefName);
            table.CreateIfNotExists();
            return table;
        }

    }
}