using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderDeliveryTypetModel
    {
        public List<DeliveryType> deliveryTypes { get; set; }
    }
    public class DeliveryType
    {
        public long id { get; set; }
        public string tpye { get; set; }
    }
}