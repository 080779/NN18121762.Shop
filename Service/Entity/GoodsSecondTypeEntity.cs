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
    /// 商品二级分类实体类
    /// </summary>
    public class GoodsSecondTypeEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public long GoodsTypeId { get; set; }
        public virtual GoodsTypeEntity GoodsType { get; set; }
        public bool IsNull { get; set; } = false;
    }
}
