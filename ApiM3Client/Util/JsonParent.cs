using ApiM3Connector.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiM3Connector.Util
{
    public class JsonParent
    {
        public string RowIndex { get; set; }

        public JsonChild[] NameValue { get; set; }
    }
}
