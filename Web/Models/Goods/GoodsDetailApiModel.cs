using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Goods
{
    public class GoodsDetailApiModel
    {
        public List<GoodsImg> goodsImgs { get; set; }
        public long id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal realityPrice { get; set; }
        public string description { get; set; }
        public long inventory { get; set; }//库存
        public long saleNum { get; set; }
    }
    public class GoodsImg
    {
        public long id { get; set; }
        public string imgUrl { get; set; }
    }
}