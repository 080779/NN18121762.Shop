using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class BonusRatioDTO : BaseDTO
    {
        public long GoodsId { get; set; }
        public decimal CommonOne { get; set; } 
        public decimal CommonTwo { get; set; }
        public decimal CommonThree { get; set; }
        public decimal GoldOne { get; set; }
        public decimal GoldTwo { get; set; }
        public decimal GoldThree { get; set; }
        public decimal PlatinumOne { get; set; }
        public decimal PlatinumTwo { get; set; }
        public decimal PlatinumThree { get; set; }
    }
}
