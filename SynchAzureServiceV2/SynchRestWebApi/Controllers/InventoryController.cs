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
                            detail = result.detail,
                            quantityAvailable = result.quantityAvailable,
                            reorderPoint = result.reorderPoint,
                            reorderQuantity = result.reorderQuantity,
                            leadTime = (int)result.leadTime,
                            location = result.location,
                            category = (int)result.category
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

                synchResponse.data = inventory;
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
                    inventories.Add(
                        new SynchInventory()
                        {
                            businessId = result.businessId,
                            name = result.name,
                            upc = result.upc,
                            defaultPrice = result.defaultPrice,
                            detail = result.detail,
                            quantityAvailable = result.quantityAvailable,
                            reorderPoint = result.reorderPoint,
                            reorderQuantity = result.reorderQuantity,
                            leadTime = (int)result.leadTime,
                            location = result.location,
                            category = (int)result.category
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
                            detail = result.detail,
                            quantityAvailable = result.quantityAvailable,
                            reorderPoint = result.reorderPoint,
                            reorderQuantity = result.reorderQuantity,
                            leadTime = (int)result.leadTime,
                            location = result.location,
                            category = (int)result.category
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
                                        newInventory.reorderPoint, newInventory.category, newInventory.location);

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
        public HttpResponseMessage Update(SynchInventory updatedInventory)
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

                if (updatedInventory.upc == null)
                    throw new WebFaultException<string>("Missing UPC for requested update operation for Inventory. Specify in payload", HttpStatusCode.BadRequest);

                SynchInventory currentInventory = null;
                var results = context.GetInventoryByUpc(businessId, updatedInventory.upc);
                IEnumerator<GetInventoryByUpcResult> inventoryEnumerator = results.GetEnumerator();
                if (inventoryEnumerator.MoveNext())
                {
                    currentInventory = new SynchInventory()
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

                // checks if any field is not provided, patch it up
                if (updatedInventory.name == null)
                    updatedInventory.name = currentInventory.name;
                if (updatedInventory.defaultPrice == null)
                    updatedInventory.defaultPrice = currentInventory.defaultPrice;
                if (updatedInventory.detail == null)
                    updatedInventory.detail = currentInventory.detail;
                if (updatedInventory.leadTime == null)
                    updatedInventory.leadTime = currentInventory.leadTime;
                if (updatedInventory.quantityAvailable == null)
                    updatedInventory.quantityAvailable = currentInventory.quantityAvailable;
                if (updatedInventory.reorderQuantity == null)
                    updatedInventory.reorderQuantity = currentInventory.reorderQuantity;
                if (updatedInventory.reorderPoint == null)
                    updatedInventory.reorderPoint = currentInventory.reorderPoint;
                if (updatedInventory.category == null)
                    updatedInventory.category = currentInventory.category;
                if (updatedInventory.location == null)
                    updatedInventory.location = currentInventory.location;

                context.UpdateInventory(businessId, updatedInventory.upc, updatedInventory.name, updatedInventory.defaultPrice,
                                        updatedInventory.detail, updatedInventory.leadTime, updatedInventory.quantityAvailable,
                                        updatedInventory.reorderQuantity, updatedInventory.reorderPoint, updatedInventory.category,
                                        updatedInventory.location);

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
    }
}
