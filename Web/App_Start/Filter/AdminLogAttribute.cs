using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.App_Start.Filter
{
    public class AdminLogAttribute:Attribute
    {
        public string AdminLog { get; set; }
        public string PermissionType { get; set; }
        public AdminLogAttribute(string PermissionType,string AdminLog)
        {
            this.AdminLog = AdminLog;
            this.PermissionType = PermissionType;
        }
    }
}