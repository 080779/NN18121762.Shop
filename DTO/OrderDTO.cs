using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class OrderDTO : BaseDTO
    {
        public string Code { get; set; }
        public long BuyerId { get; set; }
        public string BuyerNickName { get; set; }
        public string BuyerMobile { get; set; }
        public decimal Amount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? UpAmount { get; set; }
        public long PayTypeId { get; set; }
        public string PayTypeName { get; set; }//支付方式
        public int OrderStateId { get; set; }
        public string OrderStateName { get; set; }//订单状态
        public decimal PostFee { get; set; }
        public string Deliver { get; set; }
        public string DeliverName { get; set; }//物流名称
        public string DeliverCode { get; set; }//物流单号
        public string UserDeliverName { get; set; }
        public string UserDeliverCode { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiverAddress { get; set; }
        public DateTime? PayTime { get; set; }//支付时间
        public DateTime? ConsignTime { get; set; }//发货时间
        public DateTime? EndTime { get; set; }//交易完成时间
        public DateTime? CloseTime { get; set; }//交易结束时间
        public string BuyerMessage { get; set; }//买家留言
        public long? DownCycledId { get; set; }//降级处理
        public string DownCycledName { get; set; }
        public long? AuditStatusId { get; set; }//审核状态
        public string AuditStatusName { get; set; }
        public bool IsRated { get; set; } //是否评价
        public DateTime? ApplyTime { get; set; }//申请退货时间
        public decimal? DeductAmount { get; set; } //扣除金额
        public DateTime? AuditTime { get; set; }//审核时间
        public string AuditMobile { get; set; }//审核人账号
        public decimal? ReturnAmount { get; set; }//申请退货金额
        public decimal? RefundAmount { get; set; } //应退款金额
    }
}
