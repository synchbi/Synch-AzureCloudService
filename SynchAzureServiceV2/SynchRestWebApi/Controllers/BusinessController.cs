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
    public class BusinessController : ApiController
    {
        // GET api/business
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

                SynchBusiness business = getBusiness(context, businessId);

                synchResponse.data = business;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/business?
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SynchBusiness business = getBusiness(context, id);
                synchResponse.data = business;
                synchResponse.status = HttpStatusCode.OK;

            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // POST api/business
        public HttpResponseMessage Create(SynchBusiness newBusiness)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                // validate Business information
                if (String.IsNullOrEmpty(newBusiness.name) ||
                    String.IsNullOrEmpty(newBusiness.email) ||
                    String.IsNullOrEmpty(newBusiness.postalCode))
                    throw new WebFaultException<string>("name / email / postalCode does not meet business requirement", (HttpStatusCode)422);

                int businessId = context.CreateBusiness(newBusiness.name, 0, 0, newBusiness.address, newBusiness.postalCode, newBusiness.email, newBusiness.phoneNumber);
                if (businessId < 0)
                    throw new WebFaultException<string>("A Business with the same name and postal code already exists", HttpStatusCode.Conflict);

                newBusiness.id = businessId;

                Utility.EmailUtility.EmailController emailController = new Utility.EmailUtility.EmailController(businessId, 0);
                emailController.sendEmailForNewBusiness(newBusiness);

                synchResponse.data = newBusiness;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_BUSINESS, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_BUSINESS, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetSyncStatus()
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.ERP_QBD_TABLE_STATUS);

                TableOperation retrieveOperation = TableOperation.Retrieve<Utility.StorageUtility.SyncStatusEntity>("QBD", businessId.ToString());
                TableResult retrievedResult = table.Execute(retrieveOperation);
                if (retrievedResult.Result == null)
                {
                    throw new WebFaultException<string>("This business does not have a current sync status", HttpStatusCode.NotFound);
                }

                Utility.StorageUtility.SyncStatusEntity retrievedConfiguration = (Utility.StorageUtility.SyncStatusEntity)retrievedResult.Result;
                synchResponse.data = retrievedConfiguration;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpPatch]
        public HttpResponseMessage Update(int id, SynchBusiness updatedBusiness)
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

                SynchBusiness currentBusiness = getBusiness(context, businessId);

                // checks if any field is not provided, patch it up
                if (String.IsNullOrEmpty(updatedBusiness.address))
                    updatedBusiness.address = currentBusiness.address;
                if (updatedBusiness.tier == Int32.MinValue)
                    updatedBusiness.tier = currentBusiness.tier;
                if (updatedBusiness.integration == Int32.MinValue)
                    updatedBusiness.integration = currentBusiness.integration;
                if (String.IsNullOrEmpty(updatedBusiness.name))
                    updatedBusiness.name = currentBusiness.name;
                if (String.IsNullOrEmpty(updatedBusiness.phoneNumber))
                    updatedBusiness.phoneNumber = currentBusiness.phoneNumber;
                if (String.IsNullOrEmpty(updatedBusiness.postalCode))
                    updatedBusiness.postalCode = currentBusiness.postalCode;
                if (String.IsNullOrEmpty(updatedBusiness.email))
                    updatedBusiness.email = currentBusiness.email;

                context.UpdateBusiness(businessId, updatedBusiness.name, updatedBusiness.integration, updatedBusiness.tier,
                                        updatedBusiness.address, updatedBusiness.postalCode, updatedBusiness.email, updatedBusiness.phoneNumber);

                synchResponse.data = getBusiness(context, id);
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

        // DELETE api/business/5
        public void Delete(int id)
        {
        }

        private SynchBusiness getBusiness(SynchDatabaseDataContext context, int id)
        {
            SynchBusiness business = new SynchBusiness();
            var getBusinessResult = context.GetBusinessById(id);

            IEnumerator<GetBusinessByIdResult> businessResultEnum = getBusinessResult.GetEnumerator();
            if (businessResultEnum.MoveNext())
            {
                business.id = businessResultEnum.Current.id;
                business.name = businessResultEnum.Current.name;
                business.address = businessResultEnum.Current.address;
                business.email = businessResultEnum.Current.email;
                business.phoneNumber = businessResultEnum.Current.phoneNumber;
                business.postalCode = businessResultEnum.Current.postalCode;
                business.integration = (int)businessResultEnum.Current.integration;
                business.tier = (int)businessResultEnum.Current.tier;
            }
            else
                throw new WebFaultException<string>("business for this account is not found", HttpStatusCode.NotFound);

            return business;
        }
    }
}