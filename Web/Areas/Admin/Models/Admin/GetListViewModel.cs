using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.Admin
{
    public class GetListViewModel
    {
        public AdminDTO[] Admins { get; set; }
        public string PageHtml { get; set; }
    }
}