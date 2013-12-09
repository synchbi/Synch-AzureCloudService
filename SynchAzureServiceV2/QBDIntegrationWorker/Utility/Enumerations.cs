using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QBDIntegrationWorker.Utility

{
    public enum SyncStatusCode
    {
        ConnectionSuccess,
        SyncSuccess,
        SyncSkipped,

        Default,        // any enum < Default is success, any enum > Default is failure

        Started,
        ConnectionFailure,
        SyncFailure,
        NotStarted
    }

    public enum RecordCategory
    {
        Order,
        Receipt,
        PhysicalInventory,
        CycleCount,
        Return,
        QualityIssue,
        PhysicalDamage,
        SalesSample,
        Stolen
    }

    public enum RecordStatus
    {
        created,
        presented,
        sent,
        closed
    }

    public enum DeviceType
    {
        iPhone,
        Android,
        website
    }

    public enum AccountTier
    {
        sales,
        manager,
        ceo
    }

    public enum BusinessTier
    {
        free,
        standard,
        pro
    }
}