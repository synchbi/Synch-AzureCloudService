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

        public List<ERPBusinessMapEntity> localBusinessMapEntities;
        public List<ERPProductMapEntity> localProductMapEntities;
        public List<ERPRecordMapEntity> localRecordMapEntities;

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

            initializeBusinessMapEntities();
            initializeProductMapEntities();
            initializeRecordMapEntities();
        }

        public QbCredentialEntity getQbCredentialEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<QbCredentialEntity>("qbd", synchBusinessId.ToString());
            TableResult retrievedResult = credentialTable.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                QbCredentialEntity retrievedCredential = (QbCredentialEntity)retrievedResult.Result;
                return retrievedCredential;
            }
            else
                return null;
        }

        public QbConfigurationEntity getQbConfigurationEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<QbConfigurationEntity>("qbd", synchBusinessId.ToString());
            TableResult retrievedResult = configurationTable.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                QbConfigurationEntity retrievedConfiguration = (QbConfigurationEntity)retrievedResult.Result;
                return retrievedConfiguration;
            }
            else
                return null;
        }


        public void createRecordMapping(int recordId, Intuit.Ipp.Data.SalesTransaction t)
        {
            ERPRecordMapEntity newRecordMapping = new ERPRecordMapEntity(synchBusinessId, t.Id);
            newRecordMapping.rid = recordId;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newRecordMapping);
            recordMappingTable.Execute(insertOrReplaceOperation);

            localRecordMapEntities.Add(newRecordMapping);
        }

        public void createProductMapping(string upc, Intuit.Ipp.Data.Item item)
        {
            ERPProductMapEntity newProductMapping = new ERPProductMapEntity(synchBusinessId, item.Id);
            newProductMapping.upc = upc;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newProductMapping);
            productMappingTable.Execute(insertOrReplaceOperation);

            localProductMapEntities.Add(newProductMapping);
        }

        public void createBusinessMapping(int otherBusinessId, Intuit.Ipp.Data.NameBase business)
        {
            ERPBusinessMapEntity newBusinessMapping = new ERPBusinessMapEntity(synchBusinessId, business.Id);
            newBusinessMapping.idFromSynch = otherBusinessId;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newBusinessMapping);
            businessMappingTable.Execute(insertOrReplaceOperation);

            localBusinessMapEntities.Add(newBusinessMapping);
        }

        public void deleteProductMapping(ERPProductMapEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            productMappingTable.Execute(deleteOperation);

            localProductMapEntities.Remove(entity);
        }

        public void deleteBusinessMapping(ERPBusinessMapEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            businessMappingTable.Execute(deleteOperation);

            localBusinessMapEntities.Remove(entity);
        }

        public void deleteRecordMapping(ERPRecordMapEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            recordMappingTable.Execute(deleteOperation);

            localRecordMapEntities.Remove(entity);
        }

        private void initializeProductMapEntities()
        {
            TableQuery<ERPProductMapEntity> query = new TableQuery<ERPProductMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            IEnumerable<ERPProductMapEntity> entities = productMappingTable.ExecuteQuery(query);

            localProductMapEntities = entities.ToList();
        }

        private void initializeBusinessMapEntities()
        {

            TableQuery<ERPBusinessMapEntity> query = new TableQuery<ERPBusinessMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            IEnumerable<ERPBusinessMapEntity> entities = businessMappingTable.ExecuteQuery(query);

            localBusinessMapEntities = entities.ToList();
        }

        private void initializeRecordMapEntities()
        {
            TableQuery<ERPRecordMapEntity> query = new TableQuery<ERPRecordMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            IEnumerable<ERPRecordMapEntity> entities = recordMappingTable.ExecuteQuery(query);

            localRecordMapEntities = entities.ToList();
        }

        public Dictionary<string, ERPProductMapEntity> getQbItemIdToEntityMap()
        {
            Dictionary<string, ERPProductMapEntity> result = new Dictionary<string, ERPProductMapEntity>();
            List<ERPProductMapEntity> entities = localProductMapEntities;
            foreach (ERPProductMapEntity entity in entities)
            {
                // rowkey = item ID
                result.Add(entity.RowKey, entity);
            }
            return result;
        }

        public Dictionary<string, ERPBusinessMapEntity> getQbBusinessIdToEntityMap()
        {
            Dictionary<string, ERPBusinessMapEntity> result = new Dictionary<string, ERPBusinessMapEntity>();
            List<ERPBusinessMapEntity> entities = localBusinessMapEntities;
            foreach (ERPBusinessMapEntity entity in entities)
            {
                // rowkey = customer/vendor ID
                result.Add(entity.RowKey, entity);
            }
            return result;
        }

        public Dictionary<string, ERPRecordMapEntity> getQbTransactionIdToEntityMap()
        {
            Dictionary<string, ERPRecordMapEntity> result = new Dictionary<string, ERPRecordMapEntity>();
            List<ERPRecordMapEntity> entities = localRecordMapEntities;
            foreach (ERPRecordMapEntity entity in entities)
            {
                // rowkey = customer/vendor ID
                result.Add(entity.RowKey, entity);
            }
            return result;
        }
    }
}
