using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchRestWebApi.Models
{
    public interface ISynchClient
    {
        int businessId { get; set; }

        int accountId { get; set; }

        string address { get; set; }

        string email { get; set; }

        string phoneNumber { get; set; }

        Nullable<int> category { get; set; }

        // from Business table
        string name { get; set; }

        string postalCode { get; set; }

        string integrationId { get; set; }

        int status { get; set; }
    }
}
