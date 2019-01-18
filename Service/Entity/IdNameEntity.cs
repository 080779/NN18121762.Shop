using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// IdName实体类
    /// </summary>
    public class IdNameEntity : BaseEntity
    {
        public string TypeName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsNull { get; set; } = false;
    }
}
