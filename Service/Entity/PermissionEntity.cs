using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 权限实体类
    /// </summary>
    public class PermissionEntity:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long PermissionTypeId { get; set; }
        public virtual PermissionTypeEntity PermissionType { get; set; }
        public virtual ICollection<AdminEntity> Admins { get; set; } = new List<AdminEntity>();
    }
}
