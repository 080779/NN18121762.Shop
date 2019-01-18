using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class OrderListDTO : BaseDTO
    {
        public string GoodsCode { get; set; }
        public long GoodsId { get; set; }
        public string GoodsName { get; set; }
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public long Number { get; set; }
        public decimal Price { get; set; }
        public decimal RealityPrice { get; set; }
        public decimal TotalFee { get; set; }
        public string ImgUrl { get; set; }
        public bool IsReturn { get; set; }
        public long Inventory { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountFee { get; set; }
    }
}
