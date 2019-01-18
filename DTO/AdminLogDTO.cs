using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class AdminLogDTO:BaseDTO
    {
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public string Tip { get; set; }
        public long PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public long AdminId { get; set; }
        public string AdminMobile { get; set; }
    }
}
