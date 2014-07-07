using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Models
{
    public class SynchTopSku
    {
        public string upc { set; get; }

        public int totalQuantity { set; get; }
        public double revenue { set; get; }

    }
}