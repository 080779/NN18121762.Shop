using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common
{

    public class ApiResult
    {
        public int status { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
}
