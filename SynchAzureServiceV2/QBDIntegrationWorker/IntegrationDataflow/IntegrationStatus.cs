using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker.IntegrationDataflow
{
    class IntegrationStatus
    {
        public int synchBusinessId;
        
        public DateTime integrationStartTime;
        public DateTime integrationFinishTime;

        public SyncStatusCode overallSyncStatusCode;
        public SyncStatusCode invoiceSyncFromQbStatusCode;
        public SyncStatusCode invoiceSyncFromSynchStatusCode;
        public SyncStatusCode salesOrderSyncFromQbStatusCode;
        public SyncStatusCode salesOrderSyncFromSynchStatusCode;
        public SyncStatusCode customerSyncFromQbStatusCode;
        public SyncStatusCode customerSyncFromSynchStatusCode;
        public SyncStatusCode productSyncFromQbStatusCode;
        public SyncStatusCode productSyncFromSynchStatusCode;

        public string syncResultLog;

        public Exception exception;

        public IntegrationStatus(int businessId)
        {
            this.synchBusinessId = businessId;
            integrationStartTime = getCurrentTimeInPST();

            overallSyncStatusCode = SyncStatusCode.NotStarted;

            invoiceSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            invoiceSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            salesOrderSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            salesOrderSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            customerSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            customerSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            productSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            productSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            exception = null;

        }

        public void registerException(Exception e)
        {
            exception = e;

            var stackTrace = new System.Diagnostics.StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();
            var Class = methodBase.ReflectedType;
            var Namespace = Class.Namespace;

            syncResultLog += String.Format("Exception Registered {0}\nerror message: {1}\nerror from {2}\n",
                getCurrentTimeInPST().ToString(), e.Message, Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        public void registerError(string message)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();
            var Class = methodBase.ReflectedType;
            var Namespace = Class.Namespace;

            syncResultLog += String.Format("Error Registered {0}\nerror message: {1}\nerror from {2}\n",
                getCurrentTimeInPST().ToString(), message, Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        public void registerSyncResult(string message)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();
            var Class = methodBase.ReflectedType;
            var Namespace = Class.Namespace;

            syncResultLog += String.Format("Sync Result Registered {0}\nresult message: {1}\nresult from {2}\n",
                getCurrentTimeInPST().ToString(), message, Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        private DateTime getCurrentTimeInPST()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        }

        public void finish()
        {
            integrationFinishTime = getCurrentTimeInPST();
            syncResultLog += String.Format("Sync Finished for {0} at {1}", synchBusinessId.ToString(), integrationFinishTime.ToString());

            if (exception != null)
                syncResultLog += String.Format("Last exception thrown: {0}", exception.ToString());

            System.Diagnostics.Trace.TraceInformation(syncResultLog);
        }
    }
}
