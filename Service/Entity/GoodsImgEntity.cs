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
    /// 商品图片实体类
    /// </summary>
    public class GoodsImgEntity : BaseEntity
    {
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public long GoodsId { get; set; }
        public virtual GoodsEntity Goods { get; set; }
    }
}
