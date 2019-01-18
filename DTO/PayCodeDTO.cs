using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class PayCodeDTO : BaseDTO
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string CodeUrl { get; set; }
        public string Description { get; set; }
    }
}
