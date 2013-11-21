using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.ServiceModel.Web;

using SynchRestWebApi.Models;

namespace SynchRestWebApi.Utility
{
    public class TierManager
    {

        public static List<SynchRecord> filterRecordWithAccountTier(SynchDatabaseDataContext context, List<SynchRecord> originalList, int accountId)
        {
            int tier = getAccountTier(context, accountId);
            switch (tier)
            {
                case (int)AccountTier.sales:
                    List<SynchRecord> resultList = new List<SynchRecord>();
                    foreach (SynchRecord r in originalList)
                    {
                        if (r.accountId == accountId)
                            resultList.Add(r);
                    }
                    return resultList;
                case (int)AccountTier.manager:      // a placeholder for now
                    return originalList;
                case (int)AccountTier.ceo:
                    return originalList;
                default:
                    return null;
            }

        }

        /*
        public static void validateAccessToRecord(int recordAid, int recordBid, int aid, int bid, SynchDatabaseDataContext context)
        {
            // validate business access
            if (recordBid != bid)
                throw new WebFaultException<string>("record does not belong to business", HttpStatusCode.Unauthorized);

            // validate account access
            int tier = getAccountTier(context, aid);

            if (recordAid != aid)
            {
                switch (tier)
                {
                    case (int)AccountTier.sales:
                        throw new WebFaultException<string>("record does not belong to account", HttpStatusCode.Unauthorized);
                    case (int)AccountTier.manager:      // a placeholder for now
                        return;
                    case (int)AccountTier.ceo:
                        return;
                    default:
                        return;
                }   // end of switch
            }   // end of recordAid != aid
        }

        public static int countItemForBusinessWithAccount(int bid, int aid, string item, SynchDatabaseDataContext context)
        {
            int tier = getAccountTier(context, aid);
            
            switch (tier)
            {
                case (int)AccountTier.sales:
                    var salesResults = context.CountItemForBusinessWithAccount(bid, item, aid);
                    int count = 0;
                    foreach (var result in salesResults)
                    {
                        count = (int)result.Column1;
                    }
                    return count;
                case (int)AccountTier.manager:
                    var managerResults = context.CountItemForBusiness(bid, item);
                    count = 0;
                    foreach (var result in managerResults)
                    {
                        count = (int)result.Column1;
                    }
                    return count;
                case (int)AccountTier.ceo:
                    var ceoResults = context.CountItemForBusiness(bid, item);
                    count = 0;
                    foreach (var result in ceoResults)
                    {
                        count = (int)result.Column1;
                    }
                    return count;
                default:
                    return 0;
            }
            
        }

        public static List<SynchRecord> pageRecordForBusinessWithAccount(int bid, int aid, int offset, int pageSize,
            SynchDatabaseDataContext context)
        {
            int tier = getAccountTier(context, aid);
            List<SynchRecord> records = new List<SynchRecord>();
            switch (tier)
            {
                case (int)AccountTier.sales:
                    var salesResults = context.PageRecordForBusinessWithAccount(bid, pageSize, offset, aid);
                    foreach (PageRecordForBusinessWithAccountResult record in salesResults)
                    {
                        records.Add(
                            new Record()
                            {
                                id = record.id,
                                account = (int)record.account,
                                title = record.title,
                                date = (long)record.date,
                                business = (int)record.business,
                                category = (int)record.category,
                                status = (int)record.status,
                                comment = record.comment
                            });
                    }
                    return records;
                case (int)AccountTier.manager:
                    var managerResults = context.PageRecordForBusiness(bid, pageSize, offset);
                    foreach (PageRecordForBusinessResult record in managerResults)
                    {
                        records.Add(
                            new Record()
                            {
                                id = record.id,
                                account = (int)record.account,
                                title = record.title,
                                date = (long)record.date,
                                business = (int)record.business,
                                category = (int)record.category,
                                status = (int)record.status,
                                comment = record.comment
                            });
                    }
                    return records;
                case (int)AccountTier.ceo:
                    var ceoResults = context.PageRecordForBusiness(bid, pageSize, offset);
                    foreach (PageRecordForBusinessResult record in ceoResults)
                    {
                        records.Add(
                            new Record()
                            {
                                id = record.id,
                                account = (int)record.account,
                                title = record.title,
                                date = (long)record.date,
                                business = (int)record.business,
                                category = (int)record.category,
                                status = (int)record.status,
                                comment = record.comment
                            });
                    }
                    return records;
                default:
                    return null;
            }
        }
        */

        private static int getAccountTier(SynchDatabaseDataContext context, int accountId)
        {
            var results = context.GetAccountById(accountId);
            IEnumerator<GetAccountByIdResult> accountEnumerator = results.GetEnumerator();
            if (accountEnumerator.MoveNext())
            {
                GetAccountByIdResult result = accountEnumerator.Current;
                return (int)result.tier;
            }
            else
            {
                // no account found
                throw new WebFaultException<string>("account does not exist", HttpStatusCode.NotFound);
            }
        }
    }
}