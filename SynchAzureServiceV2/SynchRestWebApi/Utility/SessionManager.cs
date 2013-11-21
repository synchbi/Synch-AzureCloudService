using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace SynchRestWebApi.Utility
{
    public class SessionManager
    {
        public static int checkSession(SynchDatabaseDataContext context, int accountId, string sessionId)
        {
            var results = context.GetAccountById(accountId);
            IEnumerator<GetAccountByIdResult> accountEnum = results.GetEnumerator();
            if (accountEnum.MoveNext())
            {
                if (!sessionId.Equals(accountEnum.Current.sessionId))
                {
                    throw new WebFaultException<string>("session expired", HttpStatusCode.Unauthorized);
                }

                return accountEnum.Current.businessId;
            }
            else
                throw new WebFaultException<string>("account does not exist", HttpStatusCode.NotFound);

        }
    }
}