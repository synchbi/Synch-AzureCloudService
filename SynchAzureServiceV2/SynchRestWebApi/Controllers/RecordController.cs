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
    public class RecordController : ApiController
    {
        // GET api/record
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/record/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public HttpResponseMessage Create(SynchRecord newRecord)
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

                // ignore client-sent data; server uses superior logic to overwrite
                // transaction date, order status and owner id to prevent data corruption
                newRecord.transactionDate = DateTimeOffset.Now;
                newRecord.status = (int)RecordStatus.created;
                newRecord.ownerId = businessId;

                int recordId = context.CreateRecord(
                    newRecord.category,
                    newRecord.accountId,
                    newRecord.ownerId,
                    newRecord.clientId,
                    newRecord.status,
                    newRecord.title,
                    newRecord.comment,
                    newRecord.transactionDate,
                    newRecord.deliveryDate);

                if (recordId < 0)
                    throw new WebFaultException<string>("unable to create record", HttpStatusCode.BadRequest);

                foreach (SynchRecordLine recordLine in newRecord.recordLines)
                {
                    context.CreateRecordLine(recordId, recordLine.upc, recordLine.quantity, recordLine.price, recordLine.note);
                }

                newRecord.id = recordId;
                synchResponse.data = newRecord;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/record/5
        [HttpPut]
        public HttpResponseMessage Send(int id)
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

                SynchRecord record = null;
                var recordResult = context.GetRecordById(businessId, id);
                IEnumerator<GetRecordByIdResult> recordEnumerator = recordResult.GetEnumerator();
                if (recordEnumerator.MoveNext())
                {
                    record = new SynchRecord()
                    {
                        id = recordEnumerator.Current.id,
                        accountId = recordEnumerator.Current.accountId,
                        ownerId = recordEnumerator.Current.ownerId,
                        clientId = recordEnumerator.Current.clientId,
                        status = recordEnumerator.Current.status,
                        category = recordEnumerator.Current.category,
                        title = recordEnumerator.Current.title,
                        transactionDate = recordEnumerator.Current.transactionDate,
                        deliveryDate = recordEnumerator.Current.deliveryDate,
                        comment = recordEnumerator.Current.comment
                    };
                }
                else
                {
                    throw new WebFaultException<string>("Record with given Id is not found", HttpStatusCode.NotFound);
                }

                EmailManager.sendEmailForRecord(context, record);       // TO-DO
                record.status = (int)RecordStatus.sent;
                context.UpdateRecord(id, record.status, record.title, record.comment, record.deliveryDate);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/record/5
        [HttpPut]
        public HttpResponseMessage Present(int id)
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

                SynchRecord record = null;
                var recordResult = context.GetRecordById(businessId, id);
                IEnumerator<GetRecordByIdResult> recordEnumerator = recordResult.GetEnumerator();
                if (recordEnumerator.MoveNext())
                {
                    record = new SynchRecord()
                    {
                        id = recordEnumerator.Current.id,
                        accountId = recordEnumerator.Current.accountId,
                        ownerId = recordEnumerator.Current.ownerId,
                        clientId = recordEnumerator.Current.clientId,
                        status = recordEnumerator.Current.status,
                        category = recordEnumerator.Current.category,
                        title = recordEnumerator.Current.title,
                        transactionDate = recordEnumerator.Current.transactionDate,
                        deliveryDate = recordEnumerator.Current.deliveryDate,
                        comment = recordEnumerator.Current.comment
                    };
                }
                else
                {
                    throw new WebFaultException<string>("Record with given Id is not found", HttpStatusCode.NotFound);
                }

                record.status = (int)RecordStatus.presented;
                context.UpdateRecord(id, record.status, record.title, record.comment, record.deliveryDate);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/record/5
        [HttpPatch]
        public HttpResponseMessage Update(int id, SynchRecord updatedRecord)
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

                SynchRecord currentRecord = null;
                var recordResult = context.GetRecordById(businessId, id);
                IEnumerator<GetRecordByIdResult> recordEnumerator = recordResult.GetEnumerator();
                if (recordEnumerator.MoveNext())
                {
                    currentRecord = new SynchRecord()
                    {
                        id = recordEnumerator.Current.id,
                        accountId = recordEnumerator.Current.accountId,
                        ownerId = recordEnumerator.Current.ownerId,
                        clientId = recordEnumerator.Current.clientId,
                        status = recordEnumerator.Current.status,
                        category = recordEnumerator.Current.category,
                        title = recordEnumerator.Current.title,
                        transactionDate = recordEnumerator.Current.transactionDate,
                        deliveryDate = recordEnumerator.Current.deliveryDate,
                        comment = recordEnumerator.Current.comment
                    };
                }
                else
                {
                    throw new WebFaultException<string>("Record with given Id is not found", HttpStatusCode.NotFound);
                }

                // we do not allow modification of closed record, i.e. invoice.
                if (currentRecord.status == (int)RecordStatus.closed)
                    throw new WebFaultException<string>("Record with given Id is already closed/invoiced", HttpStatusCode.Forbidden);

                // fill in record fields that are not patched
                if (updatedRecord.title == null)
                    updatedRecord.title = currentRecord.title;

                if (updatedRecord.comment == null)
                    updatedRecord.comment = currentRecord.comment;

                if (updatedRecord.deliveryDate == null)
                    updatedRecord.deliveryDate = currentRecord.deliveryDate;

                context.UpdateRecord(id, currentRecord.status, updatedRecord.title, updatedRecord.comment, updatedRecord.deliveryDate);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // DELETE api/record/5
        public void Delete(int id)
        {
        }
    }
}
