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
    /// 幻灯片实体类
    /// </summary>
    public class SlideEntity : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImgUrl { get; set; }
        public bool IsEnabled { get; set; } 
    }
}
