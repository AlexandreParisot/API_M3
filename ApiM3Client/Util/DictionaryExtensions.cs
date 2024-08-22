using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiM3Connector.Util
{

    public static class DictionaryExtensions
    {
        public static void Set(this Dictionary<string, object> keyValuePairs, string key, object value)
        {
            if (keyValuePairs != null) { if (keyValuePairs.ContainsKey(key)) keyValuePairs[key] = value; else keyValuePairs.Add(key, value); }
        }
    }

}
