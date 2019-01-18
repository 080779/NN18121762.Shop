using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common
{
    public class AjaxResult
    {
        public int Status { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}
