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
    /// 商品分类实体类
    /// </summary>
    public class GoodsTypeEntity : BaseEntity
    {
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public bool IsNull { get; set; } = false;
    }
}
