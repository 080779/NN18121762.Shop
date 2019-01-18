using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Return
{
    public class ReturnListViewModel
    {
        public OrderDTO[] Orders { get; set; }
        public long PageCount { get; set; }
        public IdNameDTO[] AuditStatus { get; set; }
    }
}