using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Models
{
    public class SynchInventory
    {

        public int businessId { get; set; }

        public string upc { get; set; }

        public string name { get; set; }

        public decimal defaultPrice { get; set; }

        public string detail { get; set; }

        public int leadTime { get; set; }

        public int quantityAvailable { get; set; }

        public int reorderQuantity { get; set; }

        public int reorderPoint { get; set; }

        public int category { get; set; }

        public string location { get; set; }
    }
}