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

using QuickBooksIntegrationWorker.IntegrationDataflow;

namespace QuickBooksIntegrationWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // Thread.Sleep(10000);
         
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("{0}: Start running QuickBooks Integration Worker Role",
                TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")).ToString());

            while (true)
            {
                // create worker threads to integrate business; each worker thread represents one distinct business
                IntegrationController qbIntegrationController = new IntegrationController(3);
                if (!qbIntegrationController.initialize())
                    continue;
                else
                {
                    qbIntegrationController.updateCustomersFromQbd();
                    qbIntegrationController.updateItemsFromQbd();
                    qbIntegrationController.updateInvoicesFromQbd();
                }
            }
        }

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
