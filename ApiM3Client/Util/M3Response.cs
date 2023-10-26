using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiM3Connector.Util
{
    public class M3Response
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public dynamic Data { get; set; }

        public Dictionary<string, string> M3Records { get; set; }

        public string DataRaw { get; set; }
        public M3Response()
        {
            Success = true;
            M3Records = new Dictionary<string, string>();
        }
    }
}
