using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using System.ServiceModel.Web;

using SynchRestWebApi.Models;
using SynchRestWebApi.Utility;

namespace SynchRestWebApi.Controllers
{
    public class SynchAccountController : ApiController
    {
        // GET api/synchaccount
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/synchaccount/5
        public string Get(int id)
        {
            return "value";
        }

        public HttpResponseMessage Create(SynchAccount newAccount)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                string passwordHash = Encryptor.Generate512Hash(newAccount.password);
                string sessionId = String.Empty;
                Random rand = new Random();
                string sessionValue = string.Format("{0}", rand.Next());
                sessionId = Encryptor.GenerateSimpleHash(sessionValue);

                int accountId = context.CreateAccount(
                    newAccount.businessId,
                    newAccount.login,
                    passwordHash,
                    newAccount.tier,
                    newAccount.firstName,
                    newAccount.lastName,
                    newAccount.email,
                    newAccount.phoneNumber,
                    sessionId,
                    newAccount.deviceId);

                if (accountId < 0)
                    throw new WebFaultException<string>("login already exists", HttpStatusCode.Conflict);
                    //throw new HttpException((int)HttpStatusCode.Conflict, "login already exists");

                newAccount.id = accountId;
                newAccount.sessionId = sessionId;

                synchResponse.data = newAccount;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                context.Dispose();
            }

            return response;
        }

        // PUT api/synchaccount/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/synchaccount/5
        public void Delete(int id)
        {
        }
    }
}
