using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 返佣比例实体类
    /// </summary>
    public class BonusRatioEntity : BaseEntity
    {
        public long GoodsId { get; set; }
        public decimal CommonOne { get; set; } = 0;
        public decimal CommonTwo { get; set; } = 0;
        public decimal CommonThree { get; set; } = 0;
        public decimal GoldOne { get; set; } = 0;
        public decimal GoldTwo { get; set; } = 0;
        public decimal GoldThree { get; set; } = 0;
        public decimal PlatinumOne { get; set; } = 0;
        public decimal PlatinumTwo { get; set; } = 0;
        public decimal PlatinumThree { get; set; } = 0;
    }
}
