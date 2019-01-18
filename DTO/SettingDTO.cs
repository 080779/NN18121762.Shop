using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class SettingDTO : BaseDTO
    {
        public string TypeName { get; set; }
        public int? LevelId { get; set; }
        public string Name { get; set; }
        public string Param { get; set; }
        public string Remark { get; set; }
        public int? TypeId { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class SettingSetDTO
    {
        public long Id { get; set; }
        public string Param { get; set; }
    }
}
