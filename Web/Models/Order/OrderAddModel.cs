using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderAddModel
    {
        public long AddressId { get; set; }
        public long PayTypeId { get; set; }
        public long GoodsId { get; set; }
        public long Number { get; set; }
    }
}