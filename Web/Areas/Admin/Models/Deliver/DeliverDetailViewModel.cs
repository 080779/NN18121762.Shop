using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Deliver
{
    public class DeliverDetailViewModel
    {
        public OrderDTO Order { get; set; }
        public OrderListDTO[] OrderList { get; set; }
    }
}