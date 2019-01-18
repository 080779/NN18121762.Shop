using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 订单确认管理接口
    /// </summary>
    public interface IOrderApplyService : IServiceSupport
    {
        Task<long> AddAsync(params GoodsCarDTO[] goodsCars);
        Task<bool> DeleteListAsync(long userId);
        Task<OrderApplySearchResult> GetModelListAsync(long userId);
    }

    public class OrderApplySearchResult
    {
        public OrderApplyDTO[] OrderApplies { get; set; }
        public decimal ToTalAmount { get; set; }
    }
}
