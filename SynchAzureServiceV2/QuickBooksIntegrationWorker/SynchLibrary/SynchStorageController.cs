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

using QuickBooksIntegrationWorker.Utility;

namespace QuickBooksIntegrationWorker.SynchLibrary
{
    class SynchStorageController
    {
        private int synchBusinessId;
        private CloudTable credentialTable;
        private CloudTable configurationTable;
        private CloudTable productMappingTable;
        private CloudTable businessMappingTable;
        private CloudTable recordMappingTable;

        public SynchStorageController(int synchBusinessId)
        {
            this.synchBusinessId = synchBusinessId;

            // set up cloud tables
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            tableClient.RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.ExponentialRetry(TimeSpan.FromSeconds(1), 5);

            credentialTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_INFO);
            configurationTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_CONFIG);
            productMappingTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_PRODUCT);
            businessMappingTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_BUSINESS);
            recordMappingTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_RECORD);

        }

        public QbCredentialEntity getQbCredentialEntity()
        {

            TableOperation retrieveOperation = TableOperation.Retrieve<QbCredentialEntity>("qbd", synchBusinessId.ToString());

            try
            {
                TableResult retrievedResult = credentialTable.Execute(retrieveOperation);
                if (retrievedResult.Result != null)
                {
                    QbCredentialEntity retrievedCredential = (QbCredentialEntity)retrievedResult.Result;
                    return retrievedCredential;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                return null;
            }
        }

        public QbConfigurationEntity getQbConfigurationEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<QbConfigurationEntity>("qbd", synchBusinessId.ToString());

            try
            {
                TableResult retrievedResult = configurationTable.Execute(retrieveOperation);
                if (retrievedResult.Result != null)
                {
                    QbConfigurationEntity retrievedConfiguration = (QbConfigurationEntity)retrievedResult.Result;
                    return retrievedConfiguration;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                return null;
            }

        }


        public void createRecordMapping(int recordId, Intuit.Ipp.Data.SalesTransaction t)
        {
            ERPRecordMapEntity newRecordMapping = new ERPRecordMapEntity(synchBusinessId, t.Id);
            newRecordMapping.rid = recordId;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newRecordMapping);

            try
            {
                // Execute the insert operation.
                recordMappingTable.Execute(insertOrReplaceOperation);
            }
            catch (Exception e)
            {
                DateTime currentDateTimePST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
                System.Diagnostics.Trace.TraceError(currentDateTimePST.ToString() + ":" + e.ToString());
                throw e;
            }
        }
    }
}
