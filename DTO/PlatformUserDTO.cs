using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class PlatformUserDTO:BaseDTO
    {
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string AdderMobile { get; set; }
        public string Description { get; set; }
        public long PlatformIntegral { get; set; }
        public long GivingIntegral { get; set; }
        public long UseIntegral { get; set; }
        public int ErrorCount { get; set; }
        public DateTime ErrorTime { get; set; }
        public bool IsEnabled { get; set; }
        public long PlatformUserTypeId { get; set; }
        public string  PlatformUserTypeName { get; set; }
    }
}
