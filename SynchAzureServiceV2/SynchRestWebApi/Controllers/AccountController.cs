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
    public class AccountController : ApiController
    {
        // GET api/account
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/account/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
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
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Login(SynchAccount account)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                int deviceType = Int32.Parse(RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.DEVICE_TYPE)));

                string passwordHash = Encryptor.Generate512Hash(account.password);
                var results = context.GetAccountByLogin(account.login);

                IEnumerator<GetAccountByLoginResult> resultEnum = results.GetEnumerator();
                if (resultEnum.MoveNext())
                {
                    // check if it is the correct password
                    if (resultEnum.Current.password != passwordHash)
                        throw new WebFaultException<string>("unmatched credential", HttpStatusCode.Unauthorized);

                    string sessionId = String.Empty;
                    if (deviceType != (int)DeviceType.website)
                    {
                        Random rand = new Random();
                        string sessionValue = string.Format("{0}", rand.Next());
                        sessionId = Encryptor.GenerateSimpleHash(sessionValue);

                        context.UpdateAccountSession(resultEnum.Current.id, sessionId);
                    }
                    else
                    {
                        sessionId = resultEnum.Current.sessionId;
                    }

                    account.id = resultEnum.Current.id;
                    account.firstName = resultEnum.Current.firstName;
                    account.lastName = resultEnum.Current.lastName;
                    account.businessId = resultEnum.Current.businessId;
                    account.deviceId = resultEnum.Current.deviceId;
                    account.email = resultEnum.Current.email;
                    account.tier = (int)resultEnum.Current.tier;
                    account.phoneNumber = resultEnum.Current.phoneNumber;
                    account.sessionId = resultEnum.Current.sessionId;

                    synchResponse.data = account;
                    synchResponse.status = HttpStatusCode.OK;

                }
                else
                {
                    // login is wrong
                    throw new WebFaultException<string>("unmatched credential", HttpStatusCode.NotFound);
                }
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/account/5
        public void Delete(int id)
        {
        }
    }
}
