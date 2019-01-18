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
    /// 管理员实体类
    /// </summary>
    public class AdminEntity:BaseEntity
    {
        public string Mobile { get; set; }
        public string Description { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; } = true;
        public virtual ICollection<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
    }
}
