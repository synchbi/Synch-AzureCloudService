using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace QBDIntegrationWorker.Utility
{
    class MessageController
    {
        public static List<string> retrieveMessageFromSynchStorage()
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(ApplicationConstants.ERP_QBD_QUEUE);
            queue.CreateIfNotExists();
            CloudQueueMessage retrievedMessage = queue.GetMessage();

            List<string> messages = new List<string>();
            while (retrievedMessage != null)
            {
                //Process the message in less than 30 seconds, and then delete the message
                string message = retrievedMessage.AsString;
                queue.DeleteMessage(retrievedMessage);
                messages.Add(message);
                retrievedMessage = queue.GetMessage();
            }

            return messages;
        }

    }
}
