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

                var results = context.GetRecords(businessId);

                List<SynchRecord> records = new List<SynchRecord>();
                foreach (var result in results)
                {
                    records.Add(
                        new SynchRecord()
                        {
                            id = result.id,
                            accountId = result.accountId,
                            ownerId = result.ownerId,
                            clientId = result.clientId,
                            status = result.status,
                            category = result.category,
                            title = result.title,
                            transactionDate = result.transactionDate,
                            deliveryDate = result.deliveryDate,
                            comment = result.comment
                        }
                    );
                }

                foreach (SynchRecord record in records)
                {
                    record.recordLines = getRecordLines(context, businessId, record.id);
                }

                synchResponse.data = records;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/record?id={id}
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

                SynchRecord record = getCompleteRecord(context, id, businessId);                
                
                synchResponse.data = record;
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

        [HttpGet]
        public HttpResponseMessage Search(string query)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                throw new WebFaultException<string>("not implemented", HttpStatusCode.NotImplemented);
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

        [HttpGet]
        public HttpResponseMessage Page(int page, int size)
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

                var results = context.PageRecords(businessId, size, page * size);

                List<SynchRecord> records = new List<SynchRecord>();
                foreach (var result in results)
                {
                    records.Add(
                        new SynchRecord()
                        {
                            id = result.id,
                            accountId = result.accountId,
                            ownerId = result.ownerId,
                            clientId = result.clientId,
                            status = result.status,
                            category = result.category,
                            title = result.title,
                            transactionDate = result.transactionDate,
                            deliveryDate = result.deliveryDate,
                            comment = result.comment
                        }
                    );
                }

                foreach (SynchRecord record in records)
                {
                    record.recordLines = getRecordLines(context, businessId, record.id);
                }

                synchResponse.pagination = new SynchHttpResponseMessage.SynchPagination(page, size, "api/record");
                synchResponse.data = records;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpGet]
        public HttpResponseMessage Filter(int page = 0, int size = Int32.MaxValue, int accountFilter = -1, int clientFilter = -1, int statusFilter = -1, int categoryFilter = -1)
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

                var results = context.GetRecords(businessId);

                List<SynchRecord> records = new List<SynchRecord>();
                foreach (var result in results)
                {
                    records.Add(
                        new SynchRecord()
                        {
                            id = result.id,
                            accountId = result.accountId,
                            ownerId = result.ownerId,
                            clientId = result.clientId,
                            status = result.status,
                            category = result.category,
                            title = result.title,
                            transactionDate = result.transactionDate,
                            deliveryDate = result.deliveryDate,
                            comment = result.comment
                        }
                    );
                }

                // filter first
                IEnumerable<SynchRecord> filteredRecords = records.Where(
                    r =>    (accountFilter > 0 ? r.accountId == accountFilter : true) &&
                            (clientFilter > 0 ? r.clientId == clientFilter : true) &&
                            (statusFilter >= 0 ? r.status == statusFilter : true) &&
                            (categoryFilter >= 0 ? r.category == categoryFilter : true)).Skip(page * size).Take(size);

                foreach (SynchRecord record in filteredRecords)
                {
                    record.recordLines = getRecordLines(context, businessId, record.id);
                }

                synchResponse.pagination = new SynchHttpResponseMessage.SynchPagination(page, size, "api/record");
                synchResponse.data = filteredRecords;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
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
                newRecord.accountId = accountId;

                validateRecord(newRecord);

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
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
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

                SynchRecord record = getCompleteRecord(context, id, businessId);

                updateInventoryLevelsFromRecord(context, record);

                EmailManager.sendEmailForRecord(context, record);       // TO-DO
                MessageManager.sendMessageForSendRecord(record, context);
                record.status = (int)RecordStatus.sent;
                context.UpdateRecord(id, record.status, record.title, record.comment, record.deliveryDate);

                synchResponse.data = getCompleteRecord(context, id, businessId);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
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

                synchResponse.data = getCompleteRecord(context, id, businessId);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
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

                SynchRecord currentRecord = getCompleteRecord(context, id, businessId);

                // we do not allow modification of closed record, i.e. invoice.
                if (currentRecord.status == (int)RecordStatus.closed)
                    throw new WebFaultException<string>("Record with given Id is already closed/invoiced", HttpStatusCode.Forbidden);

                // fill in record fields that are not patched
                if (String.IsNullOrEmpty(updatedRecord.title))
                    updatedRecord.title = currentRecord.title;

                if (String.IsNullOrEmpty(updatedRecord.comment))
                    updatedRecord.comment = currentRecord.comment;

                if (updatedRecord.deliveryDate == null)
                    updatedRecord.deliveryDate = currentRecord.deliveryDate;

                context.UpdateRecord(id, currentRecord.status, updatedRecord.title, updatedRecord.comment, updatedRecord.deliveryDate);

                // update record line items
                if (updatedRecord.recordLines != null)
                {
                    context.DeleteRecordLinesById(updatedRecord.id);
                    foreach (SynchRecordLine recordLine in updatedRecord.recordLines)
                    {
                        context.CreateRecordLine(updatedRecord.id, recordLine.upc, recordLine.quantity, recordLine.price, recordLine.note);
                    }
                }

                synchResponse.data = getCompleteRecord(context, id, businessId);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
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

        private SynchRecord getCompleteRecord(SynchDatabaseDataContext context, int recordId, int businessId)
        {
            SynchRecord record = null;
            var recordResult = context.GetRecordById(businessId, recordId);
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
                    comment = recordEnumerator.Current.comment,
                };
            }
            else
            {
                throw new WebFaultException<string>("Record with given Id is not found", HttpStatusCode.NotFound);
            }

            // get line items
            record.recordLines = getRecordLines(context, businessId, recordId);

            return record;
        }

        private List<SynchRecordLine> getRecordLines(SynchDatabaseDataContext context, int businessId, int recordId)
        {
            // get line items
            var linesResults = context.GetRecordLines(recordId, businessId);
            List<SynchRecordLine> recordLines = new List<SynchRecordLine>();
            foreach (var lineResult in linesResults)
            {
                recordLines.Add(new SynchRecordLine()
                {
                    recordId = recordId,
                    upc = lineResult.upc,
                    quantity = lineResult.quantity,
                    price = lineResult.price,
                    note = lineResult.note
                });
            }

            return recordLines;
        }

        private void validateRecord(SynchRecord record)
        {

            // check record logic
            if (record.ownerId == record.clientId &&
                (record.category == (int)RecordCategory.Order || record.category == (int)RecordCategory.Receipt))
                throw new WebFaultException<string>("Logical error: unable to create order/receipt with same owner and client; please submit as inventory change", (HttpStatusCode)422);

            if (record.deliveryDate == null &&
                (record.category == (int)RecordCategory.Order || record.category == (int)RecordCategory.Receipt))
                throw new WebFaultException<string>("Logical error: unable to create order/receipt with no delivery date", (HttpStatusCode)422);

            if (record.recordLines == null || record.recordLines.Count() < 1)
                throw new WebFaultException<string>("Logical error: unable to create record with no line item", (HttpStatusCode)422);

            if (record.category != (int)RecordCategory.Order && record.category != (int)RecordCategory.Receipt
                && record.ownerId != record.clientId)
                throw new WebFaultException<string>("Logical error: unable to create an inventory change with different owner and client", (HttpStatusCode)422);

            if (record.category == (int)RecordCategory.Stolen || record.category == (int)RecordCategory.PhysicalDamage || record.category == (int)RecordCategory.QualityIssue)
            {
                foreach (SynchRecordLine line in record.recordLines)
                {
                    if (line.quantity > 0)
                        throw new WebFaultException<string>("Logical error: stolen quantity cannot be negative", (HttpStatusCode)422);
                }
            }

            if (record.category == (int)RecordCategory.Return)
            {
                foreach (SynchRecordLine line in record.recordLines)
                {
                    if (line.quantity < 0)
                        throw new WebFaultException<string>("Logical error: stolen quantity cannot be negative", (HttpStatusCode)422);
                }
            }
        }

        private void updateInventoryLevelsFromRecord(SynchDatabaseDataContext context, SynchRecord record)
        {
            switch (record.category)
            {
                case (int)RecordCategory.Order:
                    foreach (SynchRecordLine line in record.recordLines)
                    {
                        SynchInventory currentInventory = getInventory(context, record.ownerId, line.upc);

                        // In the future implement reorder alarm logic and negative inventory logic here
                        int newInventoryLevel = currentInventory.quantityAvailable - line.quantity;
                        context.UpdateInventoryLevel(currentInventory.businessId, currentInventory.upc, newInventoryLevel);
                    }
                    break;
                case (int)RecordCategory.Receipt:
                    foreach (SynchRecordLine line in record.recordLines)
                    {
                        SynchInventory currentInventory = getInventory(context, record.ownerId, line.upc);

                        // In the future implement reorder alarm logic and negative inventory logic here
                        int newInventoryLevel = currentInventory.quantityAvailable + line.quantity;
                        context.UpdateInventoryLevel(currentInventory.businessId, currentInventory.upc, newInventoryLevel);
                    }
                    break;
                default:
                    foreach (SynchRecordLine line in record.recordLines)
                    {
                        SynchInventory currentInventory = getInventory(context, record.ownerId, line.upc);

                        // In the future implement reorder alarm logic and negative inventory logic here
                        int newInventoryLevel = currentInventory.quantityAvailable + line.quantity;
                        context.UpdateInventoryLevel(currentInventory.businessId, currentInventory.upc, newInventoryLevel);
                    }
                    break;
            }
            
        }

        private SynchInventory getInventory(SynchDatabaseDataContext context, int businessId, string upc)
        {
            var results = context.GetInventoryByUpc(businessId, upc);
            SynchInventory inventory = null;
            IEnumerator<GetInventoryByUpcResult> inventoryEnumerator = results.GetEnumerator();
            if (inventoryEnumerator.MoveNext())
            {
                inventory = new SynchInventory()
                {
                    businessId = inventoryEnumerator.Current.businessId,
                    name = inventoryEnumerator.Current.name,
                    upc = inventoryEnumerator.Current.upc,
                    defaultPrice = inventoryEnumerator.Current.defaultPrice,
                    detail = inventoryEnumerator.Current.detail,
                    quantityAvailable = inventoryEnumerator.Current.quantityAvailable,
                    reorderPoint = inventoryEnumerator.Current.reorderPoint,
                    reorderQuantity = inventoryEnumerator.Current.reorderQuantity,
                    leadTime = (int)inventoryEnumerator.Current.leadTime,
                    location = inventoryEnumerator.Current.location,
                    category = (int)inventoryEnumerator.Current.category
                };
            }
            else
            {
                throw new WebFaultException<string>("Inventory with given UPC is not found in your Inventory", HttpStatusCode.NotFound);
            }

            return inventory;
        }

    }
}
