using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Utility
{

    public enum RecordCategory
    {
        Order,                  // down
        Receipt,                // up
        PhysicalInventory,      // up or down
        CycleCount,             // up or down
        Return,                 // up
        QualityIssue,           // down
        PhysicalDamage,         // down
        SalesSample,            // up or down
        Stolen                  // down
    }

    public enum InventoryStatus
    {
        inactive,
        active
    }

    public enum CustomerStatus
    {
        inactive,
        active
    }

    public enum SupplierStatus
    {
        inactive,
        active
    }

    public enum RecordStatus
    {
        created,
        presented,
        sentFromSynch,
        syncedSalesOrder,
        syncedInvoice,
        closed,
        rejected
    }

    public enum RecordMessageStatus
    {
        sentToIntegration,
        processedByIntegration,
        sentToErp,
        failSendToErp
    }

    public enum DeviceType
    {
        iPhone,
        Android,
        website
    }

    public enum BusinessTier
    {
        free,
        standard,
        pro
    }

    public enum CrossRoleAction
    {
        create,
        read,
        update,
        delete
    }
}