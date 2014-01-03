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

        private static CloudTable setupTable(string tableRefName)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            tableClient.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(1), 5);

            CloudTable table = tableClient.GetTableReference(tableRefName);

            return table;
        }

        public static SyncStatusEntity getSyncStatusEntity(int businessId)
        {
            CloudTable table = setupTable(ApplicationConstants.ERP_QBD_TABLE_STATUS);

            TableOperation retrieveOperation = TableOperation.Retrieve<SyncStatusEntity>("QBD", businessId.ToString());
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                SyncStatusEntity retrievedConfiguration = (SyncStatusEntity)retrievedResult.Result;
                return retrievedConfiguration;
            }
            else
                return null;
        }
    }
}