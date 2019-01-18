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
    /// 管理员操作日志管理类
    /// </summary>
    public class AdminLogEntity:BaseEntity
    {
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public string Tip { get; set; }
        public long PermissionTypeId { get; set; }
        public long AdminId { get; set; }
        public virtual PermissionTypeEntity PermissionType { get; set; }
        public virtual AdminEntity Admin { get; set; }
    }
}
