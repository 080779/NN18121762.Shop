using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 权限类型实体
    /// </summary>
    public class PermissionTypeEntity:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsNull { get; set; } = false;
    }
}
