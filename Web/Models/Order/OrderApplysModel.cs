using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderApplysModel
    {
        public long AddressId { get; set; }
        public int PayTypeId { get; set; }
        public int DeliveryTypeId { get; set; } = 1;
    }
}