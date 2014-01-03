using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker.IntegrationDataflow
{
    class IntegrationConfiguration
    {
        public string timezone;
        public DateTime historyStartDate;
        public int defaultAccountId;
        public bool syncOrderAsInvoice;
        public bool isInitialSync;
        public bool resyncHistory;
        public bool resyncInventory;
        public bool resyncCustomer;

        public IntegrationConfiguration(QbConfigurationEntity configurationEntity)
        {
            timezone = configurationEntity.timezone;
            syncOrderAsInvoice = configurationEntity.syncOrderAsInvoice;

            isInitialSync = !configurationEntity.initialized;

            if (isInitialSync || configurationEntity.resyncHistory)
                historyStartDate = new DateTime(2013, 8, 1);
            else
                historyStartDate = DateTime.Now.AddDays(-2);

            defaultAccountId = 9;
        }
    }
}
