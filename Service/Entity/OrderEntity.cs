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
    /// 订单实体类
    /// </summary>
    public class OrderEntity : BaseEntity
    {
        public string Code { get; set; }
        public long BuyerId { get; set; }
        public virtual UserEntity Buyer { get; set; }
        public long AddressId { get; set; }
        public virtual AddressEntity Address { get; set; }
        public decimal Amount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal? UpAmount { get; set; } = 1;//优惠比例
        public int PayTypeId { get; set; }
        //public virtual IdNameEntity PayType { get; set; }//支付方式
        public int OrderStateId { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }//支付时间
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ConsignTime { get; set; }//发货时间
        /// <summary>
        /// 交易完成时间
        /// </summary>
        public DateTime? EndTime { get; set; }//交易完成时间
        /// <summary>
        /// 交易结束时间
        /// </summary>
        public DateTime? CloseTime { get; set; }//交易结束时间
        //public virtual DeliveryEntity Delivery { get; set; }
        public decimal PostFee { get; set; } = 10;//邮费
        public string Deliver { get; set; } = "有快递单号";
        public string DeliverName { get; set; }
        public string DeliverCode { get; set; }
        public string UserDeliverName { get; set; }
        public string UserDeliverCode { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiverAddress { get; set; }
        public string BuyerMessage { get; set; }//买家留言
        public bool IsRated { get; set; } = false;//是否评价(现用于前端删除标志)
        /// <summary>
        /// 申请退货时间
        /// </summary>
        public DateTime? ApplyTime { get; set; }//申请退货时间
        /// <summary>
        /// 申请退货金额
        /// </summary>
        public decimal? ReturnAmount { get; set; }//申请退货金额
        /// <summary>
        /// 扣除金额
        /// </summary>
        public decimal? DeductAmount { get; set; } //扣除金额
        /// <summary>
        /// 应退款金额
        /// </summary>
        public decimal? RefundAmount { get; set; } //应退款金额
        /// <summary>
        /// 降级处理
        /// </summary>
        public int DownCycledId { get; set; } = 65;//降级处理
        /// <summary>
        /// 审核状态
        /// </summary>
        public int AuditStatusId { get; set; } = 65;//审核状态
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }//审核时间
        /// <summary>
        /// 审核人账号
        /// </summary>
        public string AuditMobile { get; set; }//审核人账号
    }
}
