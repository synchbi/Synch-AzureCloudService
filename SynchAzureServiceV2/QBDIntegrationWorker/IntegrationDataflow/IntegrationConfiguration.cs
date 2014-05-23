using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QBDIntegrationWorker.Utility;

namespace QBDIntegrationWorker.IntegrationDataflow
{
    public class IntegrationConfiguration
    {
        public string timezone;
        public DateTime historyStartDate;
        public int defaultAccountId;
        public bool syncOrderAsInvoice;
        public bool isInitialSync;
        public bool resyncHistory;
        public bool resyncInventory;
        public bool resyncCustomer;
        public string defaultAccountingClassId;

        public IntegrationConfiguration(QbConfigurationEntity configurationEntity)
        {
            timezone = configurationEntity.timezone;
            syncOrderAsInvoice = configurationEntity.syncOrderAsInvoice;

            isInitialSync = configurationEntity.isInitialSync;

            // TO-DO
            if (isInitialSync || configurationEntity.resyncHistory)
            {
                historyStartDate = DateTime.Now.AddMonths(-12);
            }
            else
                historyStartDate = DateTime.Now.AddDays(-35);

            //historyStartDate = new DateTime(2013, 11, 1);

            defaultAccountId = configurationEntity.defaultAccountId;
            defaultAccountingClassId = configurationEntity.defaultAccountingClassId;

        }
    }
}
