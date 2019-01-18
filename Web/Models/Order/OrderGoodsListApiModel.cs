using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderGoodsListApiModel
    {
        public IEnumerable<OrderList> goodsLists { get; set; }
        public long pageCount { get; set; }
    }
    public class OrderList
    {
        public long id { get; set; }
        public string goodsName { get; set; }
        public long orderId { get; set; }
        public string orderCode { get; set; }
        public long number { get; set; }
        public decimal price { get; set; }
        public decimal tealityPrice { get; set; }
        public decimal totalFee { get; set; }
        public string imgUrl { get; set; }
        public bool isReturn { get; set; }
        public long inventory { get; set; }
        public decimal discountFee { get; set; }
    }
}