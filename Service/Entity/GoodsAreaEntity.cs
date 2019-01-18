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
    /// 首页商品分区实体类
    /// </summary>
    public class GoodsAreaEntity : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}
