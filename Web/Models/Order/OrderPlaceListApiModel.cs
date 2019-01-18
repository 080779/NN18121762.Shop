using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderPlaceListApiModel
    {
        public List<OrderPlace> orderPlaces { get; set; }
        public decimal totalAmount { get; set; }
    }

    public class OrderPlace
    {
        public long UserId { get; set; }
        public string GoodsName { get; set; }
        public long GoodsId { get; set; }
        public long Number { get; set; }
        public decimal Price { get; set; }
        public decimal RealityPrice { get; set; }
        public decimal TotalFee { get; set; }
        public string ImgUrl { get; set; }
    }
}