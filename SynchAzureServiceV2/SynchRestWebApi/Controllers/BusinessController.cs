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
                var results = context.GetBusinessById(businessId);
                SynchBusiness business = null;
                IEnumerator<GetBusinessByIdResult> businessEnumerator = results.GetEnumerator();
                if (businessEnumerator.MoveNext())
                {
                    business = new SynchBusiness()
                    {
                        id = businessEnumerator.Current.id,
                        name = businessEnumerator.Current.name,
                        email = businessEnumerator.Current.email,
                        address = businessEnumerator.Current.address,
                        postalCode = businessEnumerator.Current.postalCode,
                        integration = (int)businessEnumerator.Current.integration,
                        phoneNumber = businessEnumerator.Current.phoneNumber,
                        tier = (int)businessEnumerator.Current.tier
                    };
                }
                else
                {
                    throw new WebFaultException<string>("Your account is not linked to an active business account", HttpStatusCode.NotFound);
                }

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
                throw new WebFaultException<string>("Not Implemented", HttpStatusCode.NotImplemented);
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
        public void Post([FromBody]string value)
        {
        }

        // PUT api/business/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/business/5
        public void Delete(int id)
        {
        }
    }
}
