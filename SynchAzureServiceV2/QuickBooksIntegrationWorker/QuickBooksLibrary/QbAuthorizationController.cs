using Intuit.Ipp.Core;
using Intuit.Ipp.Security;

namespace QuickBooksIntegrationWorker.QuickBooksLibrary
{
    /// <summary>
    /// Initializer class to return Service Context and OAuthRequestValidator
    /// for either QuickBooks Online or QuickBooks Desktop.
    /// </summary>
    internal class QbAuthorizationController
    {
        /// <summary>
        /// Helper method to initialize the OAuthValidator
        /// </summary>
        /// <param name="accessToken">Value for AccessToken</param>
        /// <param name="accessTokenSecret">Value for AccessTokenSecret</param>
        /// <param name="consumerKey">Value for ConsumerKey</param>
        /// <param name="consumerSecret">Value for ConsumerSecret</param>
        /// <returns>Object of OAuthRequestValidator</returns>
        internal static OAuthRequestValidator InitializeOAuthValidator(string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret)
        {
            OAuthRequestValidator oauthValidator =
                new OAuthRequestValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            return oauthValidator;
        }

        /// <summary>
        /// Helper method used to initialize the Service Context
        /// </summary>
        /// <param name="oauthValidator">Object of OAuthValidator</param>
        /// <param name="realmId">Value for Realm Id: unique for each company synced on Intuit Cloud</param>
        /// <param name="serviceType">enum identifier for service type: Qbd or QBO</param>
        /// <returns>service context for integrating with the company built in the context</returns>
        internal static ServiceContext InitializeServiceContext(OAuthRequestValidator oauthValidator,
            string realmId, IntuitServicesType serviceType)
        {
            ServiceContext context = null;
            context = new ServiceContext(realmId, serviceType, oauthValidator);
            return context;
        }

        /// <summary>
        /// Helper method used to initialize the Service Context
        /// </summary>
        /// <param name="oauthValidator">Object of OAuthValidator</param>
        /// <param name="appToken">Value for AppToken</param>
        /// <param name="companyId">Value for company ID</param>
        /// <param name="serviceType">enum identifier for service type: Qbd or QBO</param>
        /// <returns>service context for integrating with the company built in the context</returns>
        internal static ServiceContext InitializeServiceContext(OAuthRequestValidator oauthValidator,
            string appToken, string companyId, IntuitServicesType serviceType)
        {
            ServiceContext context = new ServiceContext(appToken, companyId, serviceType, oauthValidator);
            return context;
        }
    }
}
