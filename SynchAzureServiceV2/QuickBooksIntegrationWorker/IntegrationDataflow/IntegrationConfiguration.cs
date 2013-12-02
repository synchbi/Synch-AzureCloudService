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

        public IntegrationConfiguration(QbConfigurationEntity configurationEntity)
        {
            timezone = configurationEntity.timezone;
        }
    }
}
