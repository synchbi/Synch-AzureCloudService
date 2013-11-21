﻿using System;
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
            status = HttpStatusCode.BadRequest;
            data = null;
            error = new SynchError(-1, -1, "No Error Message Available");
            pagination = null;
        }
    }
}