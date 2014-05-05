﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker.IntegrationDataflow
{
    public class IntegrationStatus
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
        public SyncStatusCode salesRepSyncFromQbStatusCode;
        public SyncStatusCode salesRepSyncFromSynchStatusCode;

        public string syncResultLog;

        public Exception exception;

        public IntegrationStatus(int businessId)
        {
            this.synchBusinessId = businessId;
            integrationStartTime = getCurrentTimeInUTC();

            overallSyncStatusCode = SyncStatusCode.NotStarted;

            invoiceSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            invoiceSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            salesOrderSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            salesOrderSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            customerSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            customerSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            productSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            productSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

            salesRepSyncFromQbStatusCode = SyncStatusCode.NotStarted;
            salesRepSyncFromSynchStatusCode = SyncStatusCode.NotStarted;

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
                getCurrentTimeInUTC().ToString(), e.Message, Namespace + "." + Class.Name + "." + methodBase.Name);

            System.Diagnostics.Trace.TraceError(syncResultLog);
        }

        public void registerError(string message)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();
            var Class = methodBase.ReflectedType;
            var Namespace = Class.Namespace;

            syncResultLog += String.Format("Error Registered {0}\nerror message: {1}\nerror from {2}\n",
                getCurrentTimeInUTC().ToString(), message, Namespace + "." + Class.Name + "." + methodBase.Name);

            System.Diagnostics.Trace.TraceError(syncResultLog);
        }

        public void registerSyncResult(string message)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();
            var Class = methodBase.ReflectedType;
            var Namespace = Class.Namespace;

            syncResultLog += String.Format("Sync Result Registered {0}\nresult message: {1}\nresult from {2}\n",
                getCurrentTimeInUTC().ToString(), message, Namespace + "." + Class.Name + "." + methodBase.Name);
        }

        private DateTime getCurrentTimeInPST()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
        }

        private DateTime getCurrentTimeInUTC()
        {
            return DateTime.UtcNow;
        }

        public void finish()
        {
            integrationFinishTime = getCurrentTimeInUTC();
            syncResultLog += String.Format("Sync Finished for {0} at {1}", synchBusinessId.ToString(), integrationFinishTime.ToString());

            if (exception != null)
            {
                syncResultLog += String.Format("Last exception thrown: {0}", exception.ToString());
                overallSyncStatusCode = SyncStatusCode.SyncFailure;
            }
            else
            {
                overallSyncStatusCode = SyncStatusCode.SyncSuccess;
            }

            System.Diagnostics.Trace.TraceInformation(syncResultLog);
        }
    }
}
