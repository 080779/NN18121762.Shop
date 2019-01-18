using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class OrderApplyDTO : BaseDTO
    {
        public long UserId { get; set; }
        public string GoodsName { get; set; }
        public long GoodsId { get; set; }
        public long Number { get; set; }
        public decimal RealityPrice { get; set; }
        public decimal Price { get; set; }
        //public decimal PostFee { get; set; } 
        //public decimal Poundage { get; set; }
        public decimal TotalFee { get; set; }
        public string ImgUrl { get; set; }
    }
}
