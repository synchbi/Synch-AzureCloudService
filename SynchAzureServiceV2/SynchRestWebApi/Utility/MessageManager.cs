using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

using SynchRestWebApi.Models;

namespace SynchRestWebApi.Utility
{
    public static class MessageManager
    {
        // check which ERP system to integrate first
        public static void sendMessageForSendRecord(SynchRecord record, SynchDatabaseDataContext context)
        {
            switch ((RecordCategory)record.category)
            {
                case RecordCategory.Order:
                    var results = context.GetBusinessesWithIntegration(1);

                    bool isIntegratedWithERP = false;
                    foreach (var business in results)
                    {
                        if (record.ownerId == business.id)
                        {
                            isIntegratedWithERP = true;
                            break;
                        }
                    }

                    if (isIntegratedWithERP)
                    {
                        CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.ERP_QBD_TABLE_RECORD_MESSAGE);

                        Utility.StorageUtility.ERPREcordMessageEntity recordMessage = new StorageUtility.ERPREcordMessageEntity(record.ownerId, record.id);

                        recordMessage.status = (int)RecordMessageStatus.sentToIntegration;
                        recordMessage.active = true;

                        if (record.status == (int)RecordStatus.sent)        // if the record is already sent; it is an UPDATE, not a CREATE
                            recordMessage.action = (int)CrossRoleAction.update;
                        else
                            recordMessage.action = (int)CrossRoleAction.create;

                        TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(recordMessage);
                        table.Execute(insertOrReplaceOperation);

                    }
                    break;
                case RecordCategory.Receipt:
                    break;
                case RecordCategory.PhysicalInventory:
                    break;
                case RecordCategory.CycleCount:
                    break;
                case RecordCategory.Return:
                    break;
                case RecordCategory.QualityIssue:
                    break;
                case RecordCategory.PhysicalDamage:
                    break;
                case RecordCategory.SalesSample:
                    break;
                case RecordCategory.Stolen:
                    break;
                default:
                    break;
            }
            
        }
    }
}