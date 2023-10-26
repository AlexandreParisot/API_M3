using ApiM3Connector.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiM3Connector.Util
{
    public class ClientConfiguration
    {
        public string ContentType { get; set; }

        public string Accept { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Cookie { get; set; }

        public string ServiceUrl { get; set; }
    }


}
