using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class PermissionDTO:BaseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public bool IsChecked { get; set; } = false;
    }
}
