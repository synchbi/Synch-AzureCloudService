using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    [ServiceContract]
    public interface IAccountManager
    {
        #region POST Requests

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "create"
        )]
        HttpResponseMessage CreateAccount(SynchAccount newAccount);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "login"
        )]
        HttpResponseMessage Login(SynchAccount account, int deviceType);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "logout"
        )]
        HttpResponseMessage Logout(SynchAccount account);
        #endregion

        #region PUT Requests
        
        #endregion

        #region GET Requests

        #endregion

        #region DELETE Requests
        
        #endregion
    }
}
