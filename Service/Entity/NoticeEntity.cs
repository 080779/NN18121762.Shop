using IMS.Service;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 公告实体类
    /// </summary>
    public class NoticeEntity : BaseEntity
    {
        public string Code { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public DateTime FailureTime { get; set; }
        public bool IsEnabled { get; set; }
    }
}
