using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using System.ServiceModel.Web;

// for table storage
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

using SynchRestWebApi.Models;
using SynchRestWebApi.Utility;

namespace SynchRestWebApi.Controllers
{
    public class AccountController : ApiController
    {
        // GET api/account
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                int accountId = Int32.Parse(RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.ACCOUNT_ID)));
                string sessionId = RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.SESSION_ID));
                int businessId = SessionManager.checkSession(context, accountId, sessionId);

                var results = context.GetAccounts(businessId);

                List<SynchAccount> accounts = new List<SynchAccount>();
                foreach (var result in results)
                {
                    accounts.Add(
                        new SynchAccount()
                        {
                            id = result.id,
                            businessId = businessId,
                            email = result.email,
                            firstName = result.firstName,
                            lastName = result.lastName,
                            login = result.login,
                            phoneNumber = result.phoneNumber,
                            tier = (int)result.tier
                        }
                    );
                }

                synchResponse.data = accounts;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
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
                if (String.IsNullOrEmpty(newAccount.email) || String.IsNullOrEmpty(newAccount.login)
                    || newAccount.password.Length < 4)
                    throw new WebFaultException<string>("email / login / password does not meet account requirement", (HttpStatusCode)422);

                string passwordHash = Encryptor.GeneratePasswordHash_SHA512(newAccount.password);
                string sessionId = String.Empty;
                Random rand = new Random();
                string sessionValue = string.Format("{0}", rand.Next());
                sessionId = Encryptor.GererateSessionHash_MD5(sessionValue);

                if (newAccount.tier == Int32.MinValue)
                    newAccount.tier = 0;        // by default we sign user up as the most possible tier

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

                Utility.EmailUtility.EmailController emailController = new Utility.EmailUtility.EmailController(0, accountId);
                emailController.sendEmailForNewAccount(context, newAccount);

                synchResponse.data = newAccount;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
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

                string passwordHash = Encryptor.GeneratePasswordHash_SHA512(account.password);
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
                        sessionId = Encryptor.GererateSessionHash_MD5(sessionValue);

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
                    account.sessionId = sessionId;

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
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Logout()
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                int accountId = Int32.Parse(RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.ACCOUNT_ID)));
                string sessionId = RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.SESSION_ID));
                int businessId = SessionManager.checkSession(context, accountId, sessionId);

                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {

                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpPatch]
        public HttpResponseMessage Update(int id, SynchAccount updatedAccount)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                int accountId = Int32.Parse(RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.ACCOUNT_ID)));
                string sessionId = RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.SESSION_ID));
                string currentPassword = RequestHeaderReader.getFirstValueFromHeader(
                    Request.Headers.GetValues(Constants.RequestHeaderKeys.PASSWORD));
                int businessId = SessionManager.checkSession(context, accountId, sessionId);

                SynchAccount requestClientAccount = getAccount(context, accountId);
                SynchAccount currentAccount = getAccount(context, id);

                // requires correct password to update password
                if (Encryptor.GeneratePasswordHash_SHA512(currentPassword) != requestClientAccount.password)
                   throw new WebFaultException<string>("Request sender does not have correct credential to update account", HttpStatusCode.Unauthorized);
                
                // forbid modification from other business account
                if (businessId != updatedAccount.businessId)
                    throw new WebFaultException<string>("Modification of account information is forbidden from accounts outside of this business", HttpStatusCode.Forbidden);

                // checks if any field is not provided, patch it up
                if (String.IsNullOrEmpty(updatedAccount.login))
                    updatedAccount.login = currentAccount.login;
                if (updatedAccount.tier == Int32.MinValue)
                    updatedAccount.tier = currentAccount.tier;
                if (String.IsNullOrEmpty(updatedAccount.firstName))
                    updatedAccount.firstName = currentAccount.firstName;
                if (String.IsNullOrEmpty(updatedAccount.lastName))
                    updatedAccount.lastName = currentAccount.lastName;
                if (String.IsNullOrEmpty(updatedAccount.email))
                    updatedAccount.email = currentAccount.email;
                if (String.IsNullOrEmpty(updatedAccount.phoneNumber))
                    updatedAccount.phoneNumber = currentAccount.phoneNumber;

                bool updatePassword = false;
                if (String.IsNullOrEmpty(updatedAccount.password))
                    updatedAccount.password = currentAccount.password;      // this is hashed
                else
                {
                    updatePassword = true;
                }

                if (!updatePassword)
                {
                    context.UpdateAccount(id, updatedAccount.businessId, updatedAccount.login, updatedAccount.tier, updatedAccount.firstName,
                                        updatedAccount.lastName, updatedAccount.email, updatedAccount.phoneNumber,
                                        updatedAccount.password);
                }
                else
                {
                    context.UpdateAccount(id, updatedAccount.businessId, updatedAccount.login, updatedAccount.tier, updatedAccount.firstName,
                                        updatedAccount.lastName, updatedAccount.email, updatedAccount.phoneNumber,
                                        Encryptor.GeneratePasswordHash_SHA512(updatedAccount.password));
                }

                synchResponse.data = getAccount(context, id);
                ((SynchAccount)(synchResponse.data)).password = null;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_PUT, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_PUT, SynchError.SynchErrorCode.SERVICE_ACCOUNT, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // DELETE api/account/5
        public void Delete(int id)
        {

        }

        private SynchAccount getAccount(SynchDatabaseDataContext context, int id)
        {
            SynchAccount account = new SynchAccount();
            var results = context.GetAccountById(id);

            IEnumerator<GetAccountByIdResult> resultEnum = results.GetEnumerator();
            if (resultEnum.MoveNext())
            {
                account.id = resultEnum.Current.id;
                account.login = resultEnum.Current.login;
                account.password = resultEnum.Current.password;
                account.firstName = resultEnum.Current.firstName;
                account.lastName = resultEnum.Current.lastName;
                account.businessId = resultEnum.Current.businessId;
                account.deviceId = resultEnum.Current.deviceId;
                account.email = resultEnum.Current.email;
                account.tier = (int)resultEnum.Current.tier;
                account.phoneNumber = resultEnum.Current.phoneNumber;
                account.sessionId = resultEnum.Current.sessionId;
            }
            else
                throw new WebFaultException<string>("account with given id is not found", HttpStatusCode.NotFound);

            return account;
        }

        private SynchAccount getAccount(SynchDatabaseDataContext context, string login)
        {
            SynchAccount account = new SynchAccount();
            var results = context.GetAccountByLogin(login);

            IEnumerator<GetAccountByLoginResult> resultEnum = results.GetEnumerator();
            if (resultEnum.MoveNext())
            {
                account.id = resultEnum.Current.id;
                account.login = login;
                account.password = resultEnum.Current.password;
                account.firstName = resultEnum.Current.firstName;
                account.lastName = resultEnum.Current.lastName;
                account.businessId = resultEnum.Current.businessId;
                account.deviceId = resultEnum.Current.deviceId;
                account.email = resultEnum.Current.email;
                account.tier = (int)resultEnum.Current.tier;
                account.phoneNumber = resultEnum.Current.phoneNumber;
                account.sessionId = resultEnum.Current.sessionId;
            }
            else
                throw new WebFaultException<string>("account with given login is not found", HttpStatusCode.NotFound);

            return account;
        }

    }
}
