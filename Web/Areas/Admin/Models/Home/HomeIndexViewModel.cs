using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Areas.Admin.Models.Home
{
    public class HomeIndexViewModel
    {
        public PermissionTypeDTO[] PermissionTypes { get; set; }
        public string Mobile { get; set; }
        public string SysTitle { get; set; }
    }
}