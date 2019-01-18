using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class AdminDTO:BaseDTO
    {
        public string Mobile { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public long[] PermissionIds { get; set; }
    }
}
