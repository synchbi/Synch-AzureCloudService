using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Models
{
    public class SynchProfileStat
    {

        public decimal totalSale { set; get; }

        public object topProducts{set; get;}

        public object topCustomers{set; get;}

        public decimal avgOrderValue { set; get; }

        public object mostOrderCustomers { set; get; }

        public object leastOrderCustomers { set; get; }

        public int numOfPresented { set; get; }

        public int numOfInvoice { set; get; }
 
    }
}