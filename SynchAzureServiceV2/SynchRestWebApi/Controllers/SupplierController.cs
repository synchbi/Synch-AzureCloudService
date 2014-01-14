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
    public class SupplierController : ApiController
    {
        // GET api/supplier
        [HttpGet]
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

                var results = context.GetSuppliers(businessId);

                List<SynchSupplier> suppliers = new List<SynchSupplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new SynchSupplier()
                        {
                            businessId = businessId,
                            supplierId = result.supplierId,
                            accountId = (int)result.accountId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = suppliers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/supplier?id=
        [HttpGet]
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

                var results = context.GetSupplierById(businessId, id);
                SynchSupplier supplier = null;
                IEnumerator<GetSupplierByIdResult> supplierEnumerator = results.GetEnumerator();
                if (supplierEnumerator.MoveNext())
                {
                    supplier = new SynchSupplier()
                    {
                        businessId = businessId,
                        supplierId = supplierEnumerator.Current.supplierId,
                        accountId = (int)supplierEnumerator.Current.accountId,
                        name = supplierEnumerator.Current.name,
                        address = supplierEnumerator.Current.address,
                        email = supplierEnumerator.Current.email,
                        postalCode = supplierEnumerator.Current.postalCode,
                        phoneNumber = supplierEnumerator.Current.phoneNumber,
                        category = supplierEnumerator.Current.category
                    };
                }
                else
                {
                    throw new WebFaultException<string>("Supplier with given Id is not found", HttpStatusCode.NotFound);
                }

                synchResponse.data = supplier;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/supplier?id=
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
                var results = context.GetSuppliersLikeName(businessId, query);

                List<SynchSupplier> suppliers = new List<SynchSupplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new SynchSupplier()
                        {
                            businessId = businessId,
                            supplierId = result.supplierId,
                            accountId = (int)result.accountId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = suppliers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/supplier?id=
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

                var results = context.PageSuppliers(businessId, size, page * size);

                List<SynchSupplier> suppliers = new List<SynchSupplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new SynchSupplier()
                        {
                            businessId = businessId,
                            supplierId = result.supplierId,
                            accountId = (int)result.accountId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.pagination = new SynchHttpResponseMessage.SynchPagination(page, size, "api/supplier");
                synchResponse.data = suppliers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;

        }

        [HttpGet]
        public HttpResponseMessage Filter(int size, int page = 0, int accountFilter = -1, string postalCodeFilter = null)
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

                var results = context.GetSuppliers(businessId);

                List<SynchSupplier> suppliers = new List<SynchSupplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new SynchSupplier()
                        {
                            businessId = businessId,
                            supplierId = result.supplierId,
                            accountId = (int)result.accountId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                // filter first
                IEnumerable<SynchSupplier> filteredSuppliers = suppliers.Where(
                            s => (accountFilter > 0 ? s.accountId == accountFilter : true) &&
                            (!String.IsNullOrEmpty(postalCodeFilter) ? s.postalCode == postalCodeFilter : true)).Skip(page * size).Take(size);

                synchResponse.pagination = new SynchHttpResponseMessage.SynchPagination(page, size, Request.RequestUri);
                synchResponse.data = filteredSuppliers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Create(SynchSupplier newSupplier)
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

                int supplierId = context.CreateBusiness(newSupplier.name, 0, 0, newSupplier.address, newSupplier.postalCode, newSupplier.email, newSupplier.phoneNumber);
                if (supplierId < 0)
                {
                    var result = context.GetBusinessByNameAndPostalCode(newSupplier.name, newSupplier.postalCode);
                    IEnumerator<GetBusinessByNameAndPostalCodeResult> businessEnumerator = result.GetEnumerator();
                    if (businessEnumerator.MoveNext())
                    {
                        supplierId = businessEnumerator.Current.id;
                    }
                    else
                        throw new ApplicationException("failed to create new business on server, and no business with same name and postal code is found");
                }
                
                context.CreateSupplier(businessId, supplierId, newSupplier.address, newSupplier.email, newSupplier.phoneNumber, newSupplier.category, newSupplier.accountId);

                newSupplier.supplierId = supplierId;
                synchResponse.data = newSupplier;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/supplier/5
        [HttpPut]
        public void Update(int id)
        {
            int a = id;
        }


        // DELETE api/supplier/5
        [HttpDelete]
        public void Delete(int id)
        {
            int a = id;
        }
    }
}
