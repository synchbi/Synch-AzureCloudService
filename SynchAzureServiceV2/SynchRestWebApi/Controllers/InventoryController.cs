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
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
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
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
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
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_INVENTORY, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // POST api/inventory
        public void Post([FromBody]string value)
        {
        }

        // PUT api/inventory/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/inventory/5
        public void Delete(int id)
        {
        }
    }
}
