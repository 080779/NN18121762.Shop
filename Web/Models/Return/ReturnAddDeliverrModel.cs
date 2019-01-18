using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Return
{
    public class ReturnAddDeliverrModel
    {
        public long OrderId { get; set; }
        public string DeliverName { get; set; }
        public string DeliverCode { get; set; }
    }
}