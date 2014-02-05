using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

using QBDIntegrationWorker.IntegrationDataflow;
using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("{0}: Start running QuickBooks Integration Worker Role",
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")).ToString());

            
            while (true)
            {

                List<int> businessIds = getBusinessIdsWithQbdIntegration();

                foreach (int businessId in businessIds)
                {
                    //if (businessId != 2031)
                    //    continue;

                    // create worker threads to integrate business; each worker thread represents one distinct business
                    IntegrationController qbIntegrationController = new IntegrationController(businessId);
                    if (!qbIntegrationController.initialize())
                        continue;

                    qbIntegrationController.updateSalesRepsFromQb();
                    qbIntegrationController.updateCustomersFromQb();
                    qbIntegrationController.updateItemsFromQb();
                    qbIntegrationController.updateSalesOrdersFromQb();
                    qbIntegrationController.updateInvoicesFromQb();

                    List<ERPRecordMessageEntity> messages = MessageController.retrieveMessageFromSynchStorage(businessId);
                    foreach (ERPRecordMessageEntity message in messages)
                    {
                        processRecordMessage(qbIntegrationController, message);
                    }

                    qbIntegrationController.finalize();
                }
            }
        }

        #region Messaging Part

        private void processRecordMessage(IntegrationController qbIntegrationController, ERPRecordMessageEntity message)
        {

            switch ((CrossRoleAction)message.action)
            {
                case CrossRoleAction.create:      // create
                    if (1 == qbIntegrationController.createRecordInQbd(Int32.Parse(message.RowKey)))
                    {
                        message.status = (int)RecordMessageStatus.failSendToErp;
                        message.log = qbIntegrationController.integrationStatus.syncResultLog;
                    }
                    else
                    {
                        message.status = (int)RecordMessageStatus.sentToErp;
                    }
                    break;

                case CrossRoleAction.update:
                    if (1 == qbIntegrationController.updateRecordInQbd(Int32.Parse(message.RowKey)))
                    {
                        message.status = (int)RecordMessageStatus.failSendToErp;
                        message.log = qbIntegrationController.integrationStatus.syncResultLog;
                    }
                    else
                    {
                        message.status = (int)RecordMessageStatus.sentToErp;
                    }
                    break;

                default:
                    break;
            }

            MessageController.finalizeMessage(message);
        }
        #endregion

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        private List<int> getBusinessIdsWithQbdIntegration()
        {
            using (SynchDatabaseDataContext context = new SynchDatabaseDataContext())
            {
                var results = context.GetBusinessesWithIntegration(1);
                List<int> retrievedBusinessIds = new List<int>();

                foreach (var business in results)
                {
                    retrievedBusinessIds.Add(business.id);
                }

                return retrievedBusinessIds;
            }

        }
    }
}
