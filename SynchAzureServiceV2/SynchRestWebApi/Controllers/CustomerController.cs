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
    public class CustomerController : ApiController
    {
        // GET api/customer
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

                var results = context.GetCustomers(businessId);

                List<SynchCustomer> customers = new List<SynchCustomer>();
                foreach (var result in results)
                {
                    customers.Add(
                        new SynchCustomer()
                        {
                            businessId = businessId,
                            customerId = result.customerId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = customers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/customer?id=
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

                var results = context.GetCustomerById(businessId, id);
                SynchCustomer customer = null;
                IEnumerator<GetCustomerByIdResult> customerEnumerator = results.GetEnumerator();
                if (customerEnumerator.MoveNext())
                {
                    customer = new SynchCustomer()
                    {
                        businessId = businessId,
                        customerId = customerEnumerator.Current.customerId,
                        name = customerEnumerator.Current.name,
                        address = customerEnumerator.Current.address,
                        email = customerEnumerator.Current.email,
                        postalCode = customerEnumerator.Current.postalCode,
                        phoneNumber = customerEnumerator.Current.phoneNumber,
                        category = customerEnumerator.Current.category
                    };
                }
                else
                {
                    throw new WebFaultException<string>("Customer with given Id is not found", HttpStatusCode.NotFound);
                }

                synchResponse.data = customer;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/customer?id=
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
                var results = context.GetCustomersLikeName(businessId, query);

                List<SynchCustomer> customers = new List<SynchCustomer>();
                foreach (var result in results)
                {
                    customers.Add(
                        new SynchCustomer()
                        {
                            businessId = businessId,
                            customerId = result.customerId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = customers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // GET api/customer?id=
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

                var results = context.PageCustomers(businessId, size, page * size);

                List<SynchCustomer> customers = new List<SynchCustomer>();
                foreach (var result in results)
                {
                    customers.Add(
                        new SynchCustomer()
                        {
                            businessId = businessId,
                            customerId = result.customerId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.pagination = new SynchHttpResponseMessage.SynchPagination(page, size, "api/customer");
                synchResponse.data = customers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Create(SynchCustomer newCustomer)
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

                int customerId = context.CreateBusiness(newCustomer.name, 0, 0, newCustomer.address, newCustomer.postalCode, newCustomer.email, newCustomer.phoneNumber);
                if (customerId < 0)
                    throw new WebFaultException<string>("A Business with the same name and postal code already exists", HttpStatusCode.Conflict);
                context.CreateCustomer(businessId, customerId, newCustomer.address, newCustomer.email, newCustomer.phoneNumber, newCustomer.category);
                
                newCustomer.customerId = customerId;
                synchResponse.data = newCustomer;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        // PUT api/customer/5
        [HttpPut]
        public void Update(int id)
        {
            int a = id;
        }


        // DELETE api/customer/5
        [HttpDelete]
        public void Delete(int id)
        {
            int a = id;
        }
    }
}
