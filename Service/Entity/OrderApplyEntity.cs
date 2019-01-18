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
    /// 订单确认实体类
    /// </summary>
    public class OrderApplyEntity : BaseEntity
    {
        public long UserId { get; set; }
        public long GoodsId { get; set; }
        public string GoodsName { get; set; }
        public long Number { get; set; }
        public decimal Price { get; set; }
        public decimal TotalFee { get; set; }
        public string ImgUrl { get; set; }
    }
}
