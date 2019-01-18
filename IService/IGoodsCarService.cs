using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品分类管理接口
    /// </summary>
    public interface IGoodsCarService : IServiceSupport
    {
        Task<long> AddAsync(long userId,long goodsId,long num);
        Task<long> UpdateAsync(long id,long? num, bool? isSelected);
        Task<long> UpdateAsync(long userId,long goodsId, long? num,bool? isSelected);
        Task<bool> DeleteAsync(long id);
        Task<bool> DeleteListAsync(long userId);
        Task<GoodsCarDTO[]> GetModelListAsync(long? userId);
        Task<GoodsCarSearchResult> GetModelListAsync(long? userId,string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
        Task<GoodsCarSearchResult> GetModelListAsync(long? userId, bool? isSelected, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class GoodsCarSearchResult
    {
        public GoodsCarDTO[] GoodsCars { get; set; }
        public long PageCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
    }
    public class Goods
    {
        public long GoodsId { get; set; }
        public long Number { get; set; }
    }
}
