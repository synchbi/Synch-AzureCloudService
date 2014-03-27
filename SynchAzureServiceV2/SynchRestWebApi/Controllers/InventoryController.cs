using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Http.Cors;

using SynchRestWebApi.Models;
using SynchRestWebApi.Utility;

namespace SynchRestWebApi.Controllers
{
    // [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class InventoryController : ApiController
    {
        // GET api/inventory
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

                var results = context.GetInventories(businessId);

                List<SynchInventory> inventories = new List<SynchInventory>();
                foreach (var result in results)
                {
                    inventories.Add(
                        new SynchInventory()
                        {
                            businessId = result.businessId,
                            name = result.name,
                            upc = result.upc,
                            defaultPrice = result.defaultPrice,
                            purchasePrice = (decimal)result.purchasePrice,
                            detail = result.detail,
                            quantityAvailable = result.quantityAvailable,
                            quantityOnPurchaseOrder = (int)result.quantityOnPurchaseOrder,
                            reorderPoint = result.reorderPoint,
                            reorderQuantity = result.reorderQuantity,
                            leadTime = (int)result.leadTime,
                            location = result.location,
                            category = (int)result.category,
                            integrationId = result.integrationId,
                            status = result.status
                        }
                    );
                }

                synchResponse.data = inventories;
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

        // GET api/inventory?upc={upc}
        public HttpResponseMessage Get(string upc)
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

                synchResponse.data = getInventory(context, businessId, upc);
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
        public HttpResponseMessage Search(string query, int status = Int32.MinValue)
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

                query = "%" + query + "%";
                var results = context.GetInventoriesLikeName(businessId, query);

                List<SynchInventory> inventories = new List<SynchInventory>();
                foreach (var result in results)
                {
                    if (status == Int32.MinValue || status == result.status)
                    {
                        inventories.Add(
                            new SynchInventory()
                            {
                                businessId = result.businessId,
                                name = result.name,
                                upc = result.upc,
                                defaultPrice = result.defaultPrice,
                                purchasePrice = (decimal)result.purchasePrice,
                                detail = result.detail,
                                quantityAvailable = result.quantityAvailable,
                                quantityOnPurchaseOrder = (int)result.quantityOnPurchaseOrder,
                                reorderPoint = result.reorderPoint,
                                reorderQuantity = result.reorderQuantity,
                                leadTime = (int)result.leadTime,
                                location = result.location,
                                category = (int)result.category,
                                integrationId = result.integrationId,
                                status = result.status
                            }
                        );
                    }
                }

                synchResponse.data = inventories;
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

                var results = context.PageInventories(businessId, size, page * size);

                List<SynchInventory> inventories = new List<SynchInventory>();
                foreach (var result in results)
                {
                    inventories.Add(
                        new SynchInventory()
                        {
                            businessId = result.businessId,
                            name = result.name,
                            upc = result.upc,
                            defaultPrice = result.defaultPrice,
                            purchasePrice = (decimal)result.purchasePrice,
                            detail = result.detail,
                            quantityAvailable = result.quantityAvailable,
                            quantityOnPurchaseOrder = (int)result.quantityOnPurchaseOrder,
                            reorderPoint = result.reorderPoint,
                            reorderQuantity = result.reorderQuantity,
                            leadTime = (int)result.leadTime,
                            location = result.location,
                            category = (int)result.category,
                            integrationId = result.integrationId,
                            status = result.status
                        }
                    );
                }

                synchResponse.pagination = new SynchHttpResponseMessage.SynchPagination(page, size, "api/inventory");
                synchResponse.data = inventories;
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
        public HttpResponseMessage Filter(int size, int page = 0, int statusFilter = Int32.MinValue, int quantityLowerLimit = Int32.MinValue, int quantityUpperLimit = Int32.MaxValue)
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

                var results = context.GetInventories(businessId);

                List<SynchInventory> inventories = new List<SynchInventory>();
                foreach (var result in results)
                {
                    inventories.Add(
                        new SynchInventory()
                        {
                            businessId = result.businessId,
                            name = result.name,
                            upc = result.upc,
                            defaultPrice = result.defaultPrice,
                            purchasePrice = (decimal)result.purchasePrice,
                            detail = result.detail,
                            quantityAvailable = result.quantityAvailable,
                            quantityOnPurchaseOrder = (int)result.quantityOnPurchaseOrder,
                            reorderPoint = result.reorderPoint,
                            reorderQuantity = result.reorderQuantity,
                            leadTime = (int)result.leadTime,
                            location = result.location,
                            category = (int)result.category,
                            integrationId = result.integrationId,
                            status = result.status
                        }
                    );
                }

                IEnumerable<SynchInventory> filteredInventories = inventories.Where(
                            i =>    (statusFilter != Int32.MinValue ? i.status == statusFilter : true) &&
                                    (i.quantityAvailable >= quantityLowerLimit) &&
                                    (i.quantityAvailable <= quantityUpperLimit)).Skip(page * size).Take(size);


                synchResponse.pagination = new SynchHttpResponseMessage.SynchPagination(page, size, Request.RequestUri);
                synchResponse.data = filteredInventories;
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

        // POST api/inventory
        public HttpResponseMessage Create(SynchInventory newInventory)
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

                context.CreateProduct(newInventory.upc);
                context.CreateInventory(businessId, newInventory.upc, newInventory.name, newInventory.defaultPrice, newInventory.detail,
                                        newInventory.leadTime, newInventory.quantityAvailable, newInventory.reorderQuantity,
                                        newInventory.reorderPoint, newInventory.category, newInventory.location, newInventory.quantityOnPurchaseOrder,
                                        newInventory.integrationId, newInventory.status, newInventory.purchasePrice);

                synchResponse.data = newInventory;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }
        
        // PUT api/inventory/update
        [HttpPatch]
        public HttpResponseMessage Update(string upc, SynchInventory updatedInventory)
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

                // update UPC first, if neccessary
                if (!String.IsNullOrEmpty(updatedInventory.upc) && updatedInventory.upc != upc)
                {
                    var results = context.GetInventoryByUpc(businessId, updatedInventory.upc);
                    IEnumerator<GetInventoryByUpcResult> inventoryEnumerator = results.GetEnumerator();
                    if (inventoryEnumerator.MoveNext())
                    {
                        throw new WebFaultException<string>("Upc to be updated already exists in inventory", HttpStatusCode.Conflict);
                    }
                    
                    context.UpdateProductUpc(upc, updatedInventory.upc, businessId);
                    upc = updatedInventory.upc;
                }

                SynchInventory currentInventory = getInventory(context, businessId, upc);

                // checks if any field is not provided, patch it up
                if (String.IsNullOrEmpty(updatedInventory.name))
                    updatedInventory.name = currentInventory.name;
                if (updatedInventory.defaultPrice == Decimal.MinValue)
                    updatedInventory.defaultPrice = currentInventory.defaultPrice;
                if (updatedInventory.purchasePrice == Decimal.MinValue)
                    updatedInventory.purchasePrice = currentInventory.purchasePrice;
                if (String.IsNullOrEmpty(updatedInventory.detail))
                    updatedInventory.detail = currentInventory.detail;
                if (updatedInventory.leadTime == Int32.MinValue)
                    updatedInventory.leadTime = currentInventory.leadTime;
                if (updatedInventory.reorderQuantity == Int32.MinValue)
                    updatedInventory.reorderQuantity = currentInventory.reorderQuantity;
                if (updatedInventory.reorderPoint == Int32.MinValue)
                    updatedInventory.reorderPoint = currentInventory.reorderPoint;
                if (updatedInventory.category == Int32.MinValue)
                    updatedInventory.category = currentInventory.category;
                if (String.IsNullOrEmpty(updatedInventory.location))
                    updatedInventory.location = currentInventory.location;
                if (updatedInventory.status == Int32.MinValue)
                    updatedInventory.status = currentInventory.status;

                // we intentionally do not update 
                //      quantity available,
                //      quantity on P.O.,
                //      integrationId
                // from this API call
                updatedInventory.quantityAvailable = currentInventory.quantityAvailable;
                updatedInventory.quantityOnPurchaseOrder = currentInventory.quantityOnPurchaseOrder;
                updatedInventory.integrationId = currentInventory.integrationId;

                context.UpdateInventory(businessId, upc, updatedInventory.name, updatedInventory.defaultPrice,
                                        updatedInventory.detail, updatedInventory.leadTime, updatedInventory.quantityAvailable,
                                        updatedInventory.reorderQuantity, updatedInventory.reorderPoint, updatedInventory.category,
                                        updatedInventory.location, updatedInventory.quantityOnPurchaseOrder,
                                        updatedInventory.integrationId, updatedInventory.status, updatedInventory.purchasePrice);

                synchResponse.data = getInventory(context, businessId, upc);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // DELETE api/inventory/5
        public void Delete(int id)
        {
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
                    purchasePrice = (decimal)inventoryEnumerator.Current.purchasePrice,
                    detail = inventoryEnumerator.Current.detail,
                    quantityAvailable = inventoryEnumerator.Current.quantityAvailable,
                    quantityOnPurchaseOrder = (int)inventoryEnumerator.Current.quantityOnPurchaseOrder,
                    reorderPoint = inventoryEnumerator.Current.reorderPoint,
                    reorderQuantity = inventoryEnumerator.Current.reorderQuantity,
                    leadTime = (int)inventoryEnumerator.Current.leadTime,
                    location = inventoryEnumerator.Current.location,
                    category = (int)inventoryEnumerator.Current.category,
                    integrationId = inventoryEnumerator.Current.integrationId,
                    status = inventoryEnumerator.Current.status
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
