using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class GoodsCarDTO : BaseDTO
    {
        public long GoodsId { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ImgUrl { get; set; }
        public string Standard { get; set; }//规格
        public decimal Price { get; set; }
        public decimal RealityPrice { get; set; }
        public string Description { get; set; }
        public long Number { get; set; }
        public decimal GoodsAmount { get; set; }
        public long Inventory { get; set; }
        public bool IsSelected { get; set; }
    }
}
