using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 订单管理接口
    /// </summary>
    public interface IOrderService : IServiceSupport
    {
        Task<long> AddAsync(long buyerId,long addressId,int payTypeId, int orderStateId, long goodsId, long number);
        Task<long> AddAsync(long? deliveryTypeId, decimal? postFee, long buyerId, long addressId, int payTypeId, int orderStateId, params OrderApplyDTO[] orderApplies);
        Task<bool> AddUserDeliverAsync(long orderId,string deliverCode,string deliverName);
        Task<bool> UpdateAsync(long id, long? addressId, int? payTypeId, int? orderStateId);
        Task<long> UpdateDeliverStateAsync(long id, string deliver, string userDeliveryName, string userDeliveryCode);
        Task<bool> Receipt(long id, int orderStateId);
        Task<bool> FrontMarkDel(long id);
        Task<bool> DeleteAsync(long id);
        Task<OrderDTO> GetModelAsync(long id);
        Task<OrderDTO[]> GetAllAsync();
        Task<OrderSearchResult> GetModelListAsync(long? buyerId, int? orderStateId , long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<OrderSearchResult> GetRefundModelListAsync(long? buyerId, int? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<OrderSearchResult> GetReturnModelListAsync(long? buyerId, int? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<OrderSearchResult> GetDeliverModelListAsync(long? buyerId, int? orderStateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<long> ApplyReturnOrderAsync(long orderId);
        Task<long> ReturnOrderAsync(long orderId);
        Task<long> ApplyReturnAsync(long orderId);
        Task<long> ReturnAsync(long orderId);
        Task<long> RefundAuditAsync(long orderId, long adminId);
        Task<long> ReturnAuditAsync(long orderId, long adminId);
        Task AutoConfirmAsync();
        void AutoConfirm();
        Task<long> ValidOrder(long id);
        Task<decimal[]> GetTotalAmountAsync();
        Task<CalcDataModel> GetDataAsync();
        Task<bool> OrderCancelAsync(long orderId);
        Task<bool> EditReceiverInfoAsync(long id, string receiverName, string receiverMobile, string receiverAddress);
    }
    public class OrderSearchResult
    {
        public OrderDTO[] Orders { get; set; }
        public long PageCount { get; set; }
    }
    public class CalcDataModel
    {
        public int RegisterCount { get; set; }
        public int ApplyTakeCashCount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalOrderAmount { get; set; }
    }
}
