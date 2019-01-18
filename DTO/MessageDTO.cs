using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class MessageDTO : BaseDTO
    {
        public long? UserId { get; set; }
        public string Mobile { get; set; }
        public string Content { get; set; }
        public int? Flag { get; set; }
    }
}
