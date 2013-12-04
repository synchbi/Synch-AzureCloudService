using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickBooksIntegrationWorker.Utility;

namespace QuickBooksIntegrationWorker.IntegrationDataflow
{
    class IntegrationConfiguration
    {
        public string timezone;
        public DateTime historyStartDate;

        public IntegrationConfiguration(QbConfigurationEntity configurationEntity)
        {
            timezone = configurationEntity.timezone;
            if (!configurationEntity.initialized || configurationEntity.resyncHistory)
                historyStartDate = new DateTime(2013, 9, 1);
            else
                historyStartDate = DateTime.Now.AddDays(-2);
        }
    }
}
