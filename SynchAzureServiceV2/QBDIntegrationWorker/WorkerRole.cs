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
            //Thread.Sleep(10000);
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("{0}: Start running QuickBooks Integration Worker Role",
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")).ToString());

            while (true)
            {
                // create worker threads to integrate business; each worker thread represents one distinct business
                IntegrationController qbIntegrationController = new IntegrationController(1217);
                if (!qbIntegrationController.initialize())
                    continue;

                qbIntegrationController.updateSalesRepsFromQb();
                qbIntegrationController.updateCustomersFromQb();
                qbIntegrationController.updateItemsFromQb();
                qbIntegrationController.updateInvoicesFromQb();

                List<string> messages = MessageController.retrieveMessageFromSynchStorage();
                foreach (string message in messages)
                {
                    processUpdateMessage(qbIntegrationController, message);
                }

                qbIntegrationController.finalize();
            }
        }

        #region Messaging Part
        public void processUpdateMessage(IntegrationController qbIntegrationController, string message)
        {
            string[] elements = message.Split('-');
            switch (elements[0])
            {
                case "00":      // administration
                    break;
                case "01":      // business
                    break;
                case "02":      // inventory
                    break;
                case "03":      // record
                    processUpdateRecordMessage(qbIntegrationController, elements[1]);
                    break;
                default:
                    break;
            }
        }

        private void processUpdateRecordMessage(IntegrationController qbIntegrationController, string message)
        {
            string[] elements = message.Split(':');
            string operationCode = elements[0];
            int bid = Convert.ToInt32(elements[2]);
            int rid = Convert.ToInt32(elements[4]);

            switch (operationCode)
            {
                case "00":      // create
                    qbIntegrationController.createRecordInQbd(rid);
                    break;
                case "01":      // update
                    break;
                case "02":      // get
                    break;
                case "03":      // delete
                    break;
                default:
                    break;
            }
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
    }
}
