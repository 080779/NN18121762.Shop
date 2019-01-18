using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderListApiModel
    {
        public List<Order> Orders { get; set; }
        public long PageCount { get; set; }
    }
    public class Order
    {
        public long id { get; set; }
        public string code { get; set; }        
        public decimal amount { get; set; }
        public long payTypeId { get; set; }
        public string payTypeName { get; set; }//支付方式
        public long orderStateId { get; set; }
        public string orderStateName { get; set; }//订单状态
        public string deliver { get; set; }
        public string deliveryName { get; set; }//物流名称
        public string deliveryCode { get; set; }//物流单号
        public decimal postFee { get; set; }//邮费
        public DateTime createTime { get; set; }
        public string receiverName { get; set; }
        public decimal? discountAmount { get; set; }
        public string receiverAddress { get; set; }
        public string receiverMobile { get; set; }
        public DateTime? payTime { get; set; }//支付时间
        public DateTime? consignTime { get; set; }//发货时间
        public DateTime? endTime { get; set; }//交易完成时间
        public DateTime? closeTime { get; set; }//交易结束时间
        public decimal totalAmount { get; set; }
        public List<OrderGoods> OrderGoods { get; set; }
    }
    public class OrderGoods
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal realityPrice { get; set; }
        public decimal totalFee { get; set; }
        public long number { get; set; }
        public long inventory { get; set; }
        public string imgUrl { get; set; }
    }
}