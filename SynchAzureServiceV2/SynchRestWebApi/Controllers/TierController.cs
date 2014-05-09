using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using System.ServiceModel.Web;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

using SynchRestWebApi.Models;
using SynchRestWebApi.Utility;

namespace SynchRestWebApi.Controllers
{
    public class TierController : ApiController
    {
        // GET api/tier
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_ACCOUNT_TIER);

                TableQuery<SynchTier> query = new TableQuery<SynchTier>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, businessId.ToString()));

                IEnumerable<SynchTier> tiers = table.ExecuteQuery(query);

                synchResponse.data = tiers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_TIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_TIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/tier/5
        public HttpResponseMessage Get(int id)
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_ACCOUNT_TIER);

                TableOperation retrieveOperation = TableOperation.Retrieve<SynchTier>(businessId.ToString(), id.ToString());
                TableResult retrievedResult = table.Execute(retrieveOperation);
                if (retrievedResult.Result == null)
                    throw new WebFaultException<string>("Tier with given ID is not found", HttpStatusCode.NotFound);

                SynchTier tier = (SynchTier)retrievedResult.Result;
                synchResponse.data = tier;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_TIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_TIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // POST api/tier
        [HttpPost]
        public HttpResponseMessage Create(SynchTier newTier)
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_ACCOUNT_TIER);

                // get current tier ids so that we can increment 1 for the new tier
                TableQuery<SynchTier> query = new TableQuery<SynchTier>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, businessId.ToString()));

                IEnumerable<SynchTier> tiers = table.ExecuteQuery(query);
                int newTierId = -1;
                foreach (SynchTier tier in tiers)
                {
                    if (tier.id > newTierId)
                        newTierId = tier.id;
                }
                newTierId++;

                newTier.PartitionKey = businessId.ToString();
                newTier.RowKey = newTierId.ToString();
                newTier.id = newTierId;
                newTier.businessId = businessId;
                
                // input object validation check
                if (String.IsNullOrEmpty(newTier.tierName))
                    newTier.tierName = "Unnamed Tier";

                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newTier);
                table.Execute(insertOrReplaceOperation);

                newTier.id = newTierId;
                synchResponse.data = newTier;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_TIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_TIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/tier/5
        [HttpPut]
        public HttpResponseMessage Update(int id, SynchTier updatedTier)
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_ACCOUNT_TIER);

                updatedTier.PartitionKey = businessId.ToString();
                updatedTier.RowKey = id.ToString();
                updatedTier.businessId = businessId;
                updatedTier.id = id;

                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updatedTier);
                table.Execute(insertOrReplaceOperation);

                synchResponse.data = updatedTier;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_TIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_TIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // DELETE api/tier/5
        public HttpResponseMessage Delete(int id)
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

                // check if any account is still under this tier
                var results = context.GetAccounts(businessId);

                List<SynchAccount> accounts = new List<SynchAccount>();
                foreach (var result in results)
                {
                    if (result.tier == id)
                        throw new WebFaultException<string>("Cannot delete tier with account linked", HttpStatusCode.Conflict);
                }


                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_ACCOUNT_TIER);

                TableOperation retrieveOperation = TableOperation.Retrieve<SynchTier>(businessId.ToString(), id.ToString());
                TableResult retrievedResult = table.Execute(retrieveOperation);
                if (retrievedResult.Result == null)
                    throw new WebFaultException<string>("Tier with given ID is not found", HttpStatusCode.NotFound);

                SynchTier tier = (SynchTier)retrievedResult.Result;

                TableOperation deleteOperation = TableOperation.Delete(tier);

                // Execute the operation.
                table.Execute(deleteOperation);

                synchResponse.data = null;
                synchResponse.status = HttpStatusCode.NoContent;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_TIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_TIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

    }
}
