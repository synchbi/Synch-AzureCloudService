namespace SynchWebRole.ServiceManager
{
    using System;
    using SynchWebRole.Utility;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using System.ServiceModel;
    using System.Web.Script.Serialization;

    public partial class SynchDataService : ISynchDataService
    {
        public HttpRequestMessage Request { get; set; }

        public SynchDataService()
        {
            Request = new HttpRequestMessage();
            Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }
    }
}
