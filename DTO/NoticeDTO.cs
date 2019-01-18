using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class NoticeDTO : BaseDTO
    {
        public string Code { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime FailureTime { get; set; }
        public bool IsEnabled { get; set; }
    }
}
