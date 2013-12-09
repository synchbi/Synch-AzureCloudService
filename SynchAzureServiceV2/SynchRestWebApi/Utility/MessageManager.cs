using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

using SynchRestWebApi.Models;

namespace SynchRestWebApi.Utility
{
    public static class MessageManager
    {
        // check which ERP system to integrate first
        public static void sendMessageForSendRecord(SynchRecord record, SynchDatabaseDataContext context)
        {
            var results = context.GetBusinessesWithIntegration(1);

            bool isIntegratedWithERP = false;
            foreach (var business in results)
            {
                if (record.ownerId == business.id)
                {
                    isIntegratedWithERP = true;
                    break;
                }
            }

            if (isIntegratedWithERP)
            {
                CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                    Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

                // Create the queue client
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue
                CloudQueue queue = queueClient.GetQueueReference(ApplicationConstants.ERP_QBD_QUEUE);

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                // Create a message and add it to the queue
                string messageString = "03-00:bid:" + record.ownerId + ":rid:" + record.id;
                CloudQueueMessage message = new CloudQueueMessage(messageString);
                queue.AddMessage(message);
            }
        }
    }
}