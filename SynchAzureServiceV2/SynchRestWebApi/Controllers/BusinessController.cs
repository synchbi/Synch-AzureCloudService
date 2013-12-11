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
                SynchBusiness business = new SynchBusiness();
                var getBusinessResult = context.GetBusinessById(id);

                IEnumerator<GetBusinessByIdResult> businessResultEnum = getBusinessResult.GetEnumerator();
                if (businessResultEnum.MoveNext())
                {
                    business.id = businessResultEnum.Current.id;
                    business.name = businessResultEnum.Current.name;
                    business.address = businessResultEnum.Current.address;
                    business.email = businessResultEnum.Current.email;
                    business.phoneNumber = businessResultEnum.Current.phoneNumber;
                    business.postalCode = businessResultEnum.Current.postalCode;
                    business.integration = (int)businessResultEnum.Current.integration;
                    business.tier = (int)businessResultEnum.Current.tier;
                }
                else
                    throw new WebFaultException<string>("business for this account is not found", HttpStatusCode.NotFound);

                EmailManager.sendEmailForNewBusiness(business);

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

        // POST api/business
        public HttpResponseMessage Create(SynchBusiness newBusiness)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                // validate Business information
                if (String.IsNullOrEmpty(newBusiness.name) ||
                    String.IsNullOrEmpty(newBusiness.email) ||
                    String.IsNullOrEmpty(newBusiness.postalCode))
                    throw new WebFaultException<string>("name / email / postalCode does not meet business requirement", (HttpStatusCode)422);

                int businessId = context.CreateBusiness(newBusiness.name, 0, 0, newBusiness.address, newBusiness.postalCode, newBusiness.email, newBusiness.phoneNumber);
                if (businessId < 0)
                    throw new WebFaultException<string>("A Business with the same name and postal code already exists", HttpStatusCode.Conflict);

                newBusiness.id = businessId;

                EmailManager.sendEmailForNewBusiness(newBusiness);

                synchResponse.data = newBusiness;
                synchResponse.status = HttpStatusCode.Created;
            }
            catch (WebFaultException<string> e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_BUSINESS, e.Detail);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(Request, SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_BUSINESS, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
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
