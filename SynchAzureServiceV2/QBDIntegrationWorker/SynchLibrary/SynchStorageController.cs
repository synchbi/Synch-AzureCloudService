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

using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker.SynchLibrary
{
    class SynchStorageController
    {
        private int synchBusinessId;
        private CloudTable credentialTable;
        private CloudTable configurationTable;
        private CloudTable statusTable;
        private CloudTable productMappingTable;
        private CloudTable businessMappingTable;
        private CloudTable recordMappingTable;
        private CloudTable accountMappingTable;

        private Dictionary<string, ERPBusinessMapEntity> localBusinessMapEntities;
        private Dictionary<string, ERPProductMapEntity> localProductMapEntities;
        private Dictionary<string, ERPRecordMapEntity> localRecordMapEntities;
        private Dictionary<string, ERPAccountMapEntity> localAccountMapEntities;

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
            statusTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_STATUS);
            productMappingTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_PRODUCT);
            businessMappingTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_BUSINESS);
            recordMappingTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_RECORD);
            accountMappingTable = tableClient.GetTableReference(ApplicationConstants.ERP_QBD_TABLE_ACCOUNT);

            initializeBusinessMapEntities();
            initializeProductMapEntities();
            initializeRecordMapEntities();
            initializeAccountMapEntities();
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
            {
                retrieveOperation = TableOperation.Retrieve<QbCredentialEntity>("qbo", synchBusinessId.ToString());
                retrievedResult = credentialTable.Execute(retrieveOperation);
                if (retrievedResult.Result != null)
                {
                    QbCredentialEntity retrievedCredential = (QbCredentialEntity)retrievedResult.Result;
                    return retrievedCredential;
                }
                else
                    return null;
            }
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

        public void updateStatusEntity(IntegrationDataflow.IntegrationStatus status)
        {
            QbStatusEntity statusEntity = new QbStatusEntity(synchBusinessId, "QBD");
            statusEntity.integrationStartTime = status.integrationStartTime;
            statusEntity.integrationFinishTime = status.integrationFinishTime;

            statusEntity.overallSyncStatusCode = status.overallSyncStatusCode;

            statusEntity.invoiceSyncFromQbStatusCode = status.invoiceSyncFromQbStatusCode;
            statusEntity.invoiceSyncFromSynchStatusCode = status.invoiceSyncFromSynchStatusCode;

            statusEntity.productSyncFromQbStatusCode = status.productSyncFromQbStatusCode;
            statusEntity.productSyncFromSynchStatusCode = status.productSyncFromSynchStatusCode;

            statusEntity.salesOrderSyncFromQbStatusCode = status.salesOrderSyncFromQbStatusCode;
            statusEntity.salesOrderSyncFromSynchStatusCode = status.salesOrderSyncFromSynchStatusCode;

            statusEntity.salesRepSyncFromQbStatusCode = status.salesRepSyncFromQbStatusCode;
            statusEntity.salesRepSyncFromSynchStatusCode = status.salesRepSyncFromSynchStatusCode;

            statusEntity.customerSyncFromQbStatusCode = status.customerSyncFromQbStatusCode;
            statusEntity.customerSyncFromSynchStatusCode = status.customerSyncFromSynchStatusCode;

            statusEntity.syncResultLog = status.syncResultLog;

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(statusEntity);
            statusTable.Execute(insertOrReplaceOperation);

        }

        public void createRecordMapping(int recordId, Intuit.Ipp.Data.Qbd.CdmBase c)
        {
            ERPRecordMapEntity newRecordMapping = new ERPRecordMapEntity(synchBusinessId, c.Id.Value);
            newRecordMapping.rid = recordId;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newRecordMapping);
            recordMappingTable.Execute(insertOrReplaceOperation);

            localRecordMapEntities.Remove(c.Id.Value);
            localRecordMapEntities.Add(c.Id.Value, newRecordMapping);
        }

        public void createProductMapping(string upc, Intuit.Ipp.Data.Qbd.Item item)
        {
            ERPProductMapEntity newProductMapping = new ERPProductMapEntity(synchBusinessId, item.Id.Value);
            newProductMapping.upc = upc;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newProductMapping);
            productMappingTable.Execute(insertOrReplaceOperation);

            localProductMapEntities.Remove(item.Id.Value);
            localProductMapEntities.Add(item.Id.Value, newProductMapping);
        }

        public void createBusinessMapping(int otherBusinessId, Intuit.Ipp.Data.Qbd.RoleBase business)
        {
            ERPBusinessMapEntity newBusinessMapping = new ERPBusinessMapEntity(synchBusinessId, business.Id.Value);
            newBusinessMapping.idFromSynch = otherBusinessId;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newBusinessMapping);
            businessMappingTable.Execute(insertOrReplaceOperation);

            localBusinessMapEntities.Remove(business.Id.Value);
            localBusinessMapEntities.Add(business.Id.Value, newBusinessMapping);
        }

        public void createAccountMapping(int accountId, Intuit.Ipp.Data.Qbd.SalesRep salesRep)
        {
            ERPAccountMapEntity newAccountMapping = new ERPAccountMapEntity(synchBusinessId, salesRep.Id.Value);
            newAccountMapping.accountIdFromSynch = accountId;

            // Create the TableOperation that inserts the record mapping entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newAccountMapping);
            accountMappingTable.Execute(insertOrReplaceOperation);

            localAccountMapEntities.Remove(salesRep.Id.Value);
            localAccountMapEntities.Add(salesRep.Id.Value, newAccountMapping);
        }

        public void deleteProductMapping(ERPProductMapEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            productMappingTable.Execute(deleteOperation);

            localProductMapEntities.Remove(entity.RowKey);
        }

        public void deleteBusinessMapping(ERPBusinessMapEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            businessMappingTable.Execute(deleteOperation);

            localBusinessMapEntities.Remove(entity.RowKey);
        }

        public void deleteRecordMapping(ERPRecordMapEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            recordMappingTable.Execute(deleteOperation);

            localRecordMapEntities.Remove(entity.RowKey);
        }

        public void deleteAccountMapping(ERPAccountMapEntity entity)
        {
            TableOperation deleteOperation = TableOperation.Delete(entity);
            accountMappingTable.Execute(deleteOperation);

            localAccountMapEntities.Remove(entity.RowKey);
        }

        private void initializeProductMapEntities()
        {
            TableQuery<ERPProductMapEntity> query = new TableQuery<ERPProductMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            IEnumerable<ERPProductMapEntity> entities = productMappingTable.ExecuteQuery(query);

            // use lambda function to creat ea dictionary of <rowkey, entity>
            localProductMapEntities = entities.ToDictionary(x => x.RowKey, x => x);
        }

        private void initializeBusinessMapEntities()
        {

            TableQuery<ERPBusinessMapEntity> query = new TableQuery<ERPBusinessMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            IEnumerable<ERPBusinessMapEntity> entities = businessMappingTable.ExecuteQuery(query);

            localBusinessMapEntities = entities.ToDictionary(x => x.RowKey, x => x);
        }

        private void initializeRecordMapEntities()
        {
            TableQuery<ERPRecordMapEntity> query = new TableQuery<ERPRecordMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            IEnumerable<ERPRecordMapEntity> entities = recordMappingTable.ExecuteQuery(query);

            localRecordMapEntities = entities.ToDictionary(x => x.RowKey, x => x);
        }

        private void initializeAccountMapEntities()
        {
            TableQuery<ERPAccountMapEntity> query = new TableQuery<ERPAccountMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            IEnumerable<ERPAccountMapEntity> entities = accountMappingTable.ExecuteQuery(query);

            localAccountMapEntities = entities.ToDictionary(x => x.RowKey, x => x);
        }

        public Dictionary<string, ERPProductMapEntity> getQbItemIdToEntityMap()
        {
            Dictionary<string, ERPProductMapEntity> result = new Dictionary<string, ERPProductMapEntity>();
            foreach (string key in localProductMapEntities.Keys)
                result.Add(key, localProductMapEntities[key]);

            return result;
        }

        public Dictionary<string, ERPBusinessMapEntity> getQbBusinessIdToEntityMap()
        {
            Dictionary<string, ERPBusinessMapEntity> result = new Dictionary<string, ERPBusinessMapEntity>();
            foreach (string key in localBusinessMapEntities.Keys)
                result.Add(key, localBusinessMapEntities[key]);

            return result;
        }

        public Dictionary<string, ERPRecordMapEntity> getQbTransactionIdToEntityMap()
        {
            Dictionary<string, ERPRecordMapEntity> result = new Dictionary<string, ERPRecordMapEntity>();
            foreach (string key in localRecordMapEntities.Keys)
                result.Add(key, localRecordMapEntities[key]);

            return result;
        }

        public Dictionary<string, ERPAccountMapEntity> getQbSalesRepIdToEntityMap()
        {
            Dictionary<string, ERPAccountMapEntity> result = new Dictionary<string, ERPAccountMapEntity>();
            foreach (string key in localAccountMapEntities.Keys)
                result.Add(key, localAccountMapEntities[key]);

            return result;
        }
    }
}
