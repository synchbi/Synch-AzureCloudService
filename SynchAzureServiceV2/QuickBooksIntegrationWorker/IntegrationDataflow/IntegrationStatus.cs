using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickBooksIntegrationWorker.Utility;

namespace QuickBooksIntegrationWorker.IntegrationDataflow
{
    class IntegrationStatus
    {
        public int synchBusinessId;
        
        public DateTime integrationStartTime;
        public DateTime integrationFinishTime;

        public SyncStatusCode overallSyncStatusCode;
        public SyncStatusCode recordSyncStatusCode;
        public SyncStatusCode customerSyncStatusCode;
        public SyncStatusCode productSyncStatusCode;

        public string syncResultLog;

        public Exception exception;

        public IntegrationStatus(int businessId)
        {
            this.synchBusinessId = businessId;
            integrationStartTime = getCurrentTimeInPST();

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
    }
}
