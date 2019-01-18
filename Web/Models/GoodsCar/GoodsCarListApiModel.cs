using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.GoodsCar
{
    public class GoodsCarListApiModel
    {
        public List<GoodsCarModel> goodsCars { get; set; }
        public decimal totalAmount { get; set; }
    }
    public class GoodsCarModel
    {
        public long id { get; set; }
        public long goodsId { get; set; }
        public string goodsName { get; set; }
        public decimal price { get; set; }
        public decimal realityPrice { get; set; }//现价
        public long number { get; set; }
        public decimal goodsAmount { get; set; }
        public string imgUrl { get; set; }
        public bool isSelected { get; set; }
        public long inventory { get; set; }
    }
}