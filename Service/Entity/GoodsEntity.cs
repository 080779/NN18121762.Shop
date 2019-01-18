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
    /// 商品实体类
    /// </summary>
    public class GoodsEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Standard { get; set; }//规格
        public decimal Price { get; set; }//原价
        public decimal RealityPrice { get; set; }//现价
        public long GoodsAreaId { get; set; }
        //public virtual GoodsAreaEntity GoodsArea { get; set; }
        public long GoodsTypeId { get; set; }
        public virtual GoodsTypeEntity GoodsType { get; set; }
        public long GoodsSecondTypeId { get; set; }
        public virtual GoodsSecondTypeEntity GoodsSecondType { get; set; }
        public bool IsPutaway { get; set; }//是否上架
        public bool IsRecommend { get; set; }//是否推荐
        public bool IsHotSale { get; set; }
        public string Description { get; set; }
        //public long GoodsImgId { get; set; }
        //public virtual GoodsImgEntity GoodsImg { get; set; }
        public long Inventory { get; set; }//库存
        public long SaleNum { get; set; } = 0;//销售数量
    }
}
