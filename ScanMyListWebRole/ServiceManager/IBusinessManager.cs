using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    [ServiceContract]
    public interface IBusinessManager
    {
        #region GET Request
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "business?accountId={accountId}&sessionId={sessionId}"
        )]
        HttpResponseMessage GetBusiness(int accountId, string sessionId);
        #endregion
    }
}
