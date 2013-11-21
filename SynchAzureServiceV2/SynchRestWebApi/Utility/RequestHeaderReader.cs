using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchRestWebApi.Utility
{
    public static class RequestHeaderReader
    {
        public static string getFirstValueFromHeader(IEnumerable<string> enumerable)
        {
            IEnumerator<string> enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
                return enumerator.Current.ToString();
            else
                return null;
        }
    }
}