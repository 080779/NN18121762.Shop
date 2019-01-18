using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderPayModel
    {
        public long AddressId { get; set; }
        public int PayTypeId { get; set; }
        public long DeliveryTypeId { get; set; } = 0;
    }
}