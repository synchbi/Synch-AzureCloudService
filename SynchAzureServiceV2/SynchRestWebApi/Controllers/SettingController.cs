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
    public class SettingController : ApiController
    {
        // GET api/setting
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_OVERALL_SETTING);

                TableQuery<SynchSetting> query = new TableQuery<SynchSetting>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, businessId.ToString()));

                IEnumerable<SynchSetting> settings = table.ExecuteQuery(query);

                synchResponse.data = settings;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SETTING, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SETTING, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/setting/5
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_OVERALL_SETTING);

                TableOperation retrieveOperation = TableOperation.Retrieve<SynchSetting>(businessId.ToString(), id.ToString());
                TableResult retrievedResult = table.Execute(retrieveOperation);
                if (retrievedResult.Result == null)
                    throw new WebFaultException<string>("Setting with given id is not found: please note that setting has only a dummy ID to support consistent API; to get setting for this business, use businessId as ID", HttpStatusCode.NotFound);

                SynchSetting setting = (SynchSetting)retrievedResult.Result;
                synchResponse.data = setting;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SETTING, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SETTING, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // POST api/setting
        [HttpPost]
        public HttpResponseMessage Create(SynchSetting newSetting)
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_OVERALL_SETTING);

                // only keeps 1 single setting for one business, so instead of creating new one, we update current one
                // validate input
                newSetting.businessId = businessId;
                newSetting.RowKey = businessId.ToString();
                newSetting.PartitionKey = businessId.ToString();
                if (newSetting.historyStartDate <= DateTime.MinValue)
                    newSetting.historyStartDate = DateTime.Now.AddMonths(-1);
                if (String.IsNullOrEmpty(newSetting.timezone))
                    newSetting.timezone = "Pacific Standard Time";

                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newSetting);
                table.Execute(insertOrReplaceOperation);

                synchResponse.data = newSetting;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SETTING, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SETTING, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/setting/5
        [HttpPut]
        public HttpResponseMessage Update(int id, SynchSetting updatedSetting)
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

                CloudTable table = Utility.StorageUtility.StorageController.setupTable(ApplicationConstants.SYNCH_TABLE_OVERALL_SETTING);

                updatedSetting.businessId = businessId;
                updatedSetting.PartitionKey = businessId.ToString();
                updatedSetting.RowKey = businessId.ToString();

                // this is a PUT not a PATCH, so reset to default if it is null
                if (updatedSetting.historyStartDate <= DateTime.MinValue)
                    updatedSetting.historyStartDate = DateTime.Now.AddMonths(-1);
                if (String.IsNullOrEmpty(updatedSetting.timezone))
                    updatedSetting.timezone = "Pacific Standard Time";

                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updatedSetting);
                table.Execute(insertOrReplaceOperation);

                synchResponse.data = updatedSetting;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SETTING, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SETTING, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // DELETE api/setting/5
        public void Delete(int id)
        {
        }

    }
}
