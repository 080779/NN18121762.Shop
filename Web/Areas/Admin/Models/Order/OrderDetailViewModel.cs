using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Order
{
    public class OrderDetailViewModel
    {
        public OrderDTO Order { get; set; }
        public OrderListDTO[] OrderList { get; set; }
        public decimal GoodsAmount { get; set; }
    }    
}