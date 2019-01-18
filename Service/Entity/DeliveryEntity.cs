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
    /// 订单配送实体类
    /// </summary>
    public class DeliveryEntity : BaseEntity
    {
        public long OrderId { get; set; }
        public virtual OrderEntity Order { get; set; }
        public string DeliveryName { get; set; }//物流名称
        public string DeliveryCode { get; set; }//物流单号
        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiverAddress { get; set; }
    }
}
