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

namespace QBDIntegrationWorker.Utility
{
    class MessageController
    {
        public static List<ERPRecordMessageEntity> retrieveMessageFromSynchStorage(int businessId)
        {
            // Retrieve storage account from connection string
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            tableClient.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(1), 5);

            CloudTable table = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_RECORD_MESSAGE);
            table.CreateIfNotExists();

            TableQuery<ERPRecordMessageEntity> query = new TableQuery<ERPRecordMessageEntity>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, businessId.ToString()));

            IEnumerable<ERPRecordMessageEntity> messages = table.ExecuteQuery(query);

            List<ERPRecordMessageEntity> activeMessages = new List<ERPRecordMessageEntity>();

            foreach (ERPRecordMessageEntity message in messages)
            {
                if (message.active)
                    activeMessages.Add(message);
            }

            return activeMessages;
        }


        public static void finalizeMessage(ERPRecordMessageEntity message)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            tableClient.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(1), 5);

            CloudTable table = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_RECORD_MESSAGE);
            table.CreateIfNotExists();

            message.active = false;

            TableOperation replaceOperation = TableOperation.Replace(message);
            table.Execute(replaceOperation);
        }
    }
}
