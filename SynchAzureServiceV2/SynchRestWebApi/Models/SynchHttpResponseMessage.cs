using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SynchRestWebApi.Models
{
    public class SynchHttpResponseMessage
    {
        public class SynchPagination
        {
            public int pageSize { get; set; }
            public string prevPage { get; set; }
            public string nextPage { get; set; }

            public SynchPagination(int curPage, int size, string baseUri)
            {
                this.pageSize = size;
                this.prevPage = (curPage == 0) ? String.Empty : baseUri + "?page=" + (curPage - 1) + "?size=" + size;
                this.nextPage = baseUri + "?page=" + (curPage + 1) + "?size=" + size;
            }
        }

        public class SynchResponseMetaData
        {
            public DateTime timestamp { get; set; }
            public string handler { get; set; }
            public int accountId { get; set; }
            public string sessionId { get; set; }
        }

        public SynchResponseMetaData metaData { get; set; }

        public HttpStatusCode status { get; set; }

        public Object data { get; set; }

        public SynchError error { get; set; }

        public SynchPagination pagination { get; set; }

        public SynchHttpResponseMessage()
        {
            metaData = null;
            data = null;
            status = HttpStatusCode.BadRequest;
            error = new SynchError(null, -1, -1, "No Error Message Available");
            pagination = null;
        }
    }
}