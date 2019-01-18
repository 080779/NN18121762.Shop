using IMS.DTO;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Order
{
    public class OrderExportExcelModel
    {
        [ExportExcelName("编号")]
        public long Id { get; set; }
        [ExportExcelName("订单时间")]
        public string CreateTime { get; set; }
        [ExportExcelName("订单号")]
        public string Code { get; set; }
        [ExportExcelName("买家账号")]
        public string BuyerMobile { get; set; }
        [ExportExcelName("买家昵称")]
        public string BuyerNickName { get; set; }
        [ExportExcelName("收货人姓名")]
        public string ReceiverName { get; set; }
        [ExportExcelName("收货电话")]
        public string ReceiverMobile { get; set; }
        [ExportExcelName("收货地址")]
        public string ReceiverAddress { get; set; }
        [ExportExcelName("支付方式")]
        public string PayTypeName { get; set; }//支付方式
        [ExportExcelName("订单金额")]
        public decimal Amount { get; set; }
        [ExportExcelName("订单状态")]
        public string OrderStateName { get; set; }//订单状态        
    }
}