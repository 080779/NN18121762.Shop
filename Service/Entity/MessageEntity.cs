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
    /// 短信实体类
    /// </summary>
    public class MessageEntity : BaseEntity
    {
        public long? UserId { get; set; }
        public string Mobile { get; set; }
        public string Content { get; set; }
        public int? Flag { get; set; }
    }
}
