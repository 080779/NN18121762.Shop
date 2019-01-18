using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品管理接口
    /// </summary>
    public interface IGoodsService : IServiceSupport
    {
        Task<long> AddAsync(GoodsAddEditModel goods);
        Task<bool> UpdateAsync(GoodsAddEditModel goods);
        Task<bool> DeleteAsync(long id);
        Task<GoodsDTO> GetModelAsync(long id);
        Task<GoodsSearchResult> FrontModelListAsync(bool? isHotSale, long? goodsTypeId, long? goodsSecondTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<GoodsSearchResult> GetModelListAsync(bool? isPutaway, long? goodsTypeId, long? goodsSecondTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<GoodsSearchResult> SearchAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class GoodsSearchResult
    {
        public GoodsDTO[] Goods { get; set; }
        public long PageCount { get; set; }
    }
    public class GoodsAddEditModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }//原价
        public decimal RealityPrice { get; set; }//现价
        public long Inventory { get; set; }//库存
        public long GoodsAreaId { get; set; }
        public long GoodsTypeId { get; set; }
        public long? GoodsSecondTypeId { get; set; }
        public bool IsPutaway { get; set; }//是否上架
        public bool IsRecommend { get; set; }//是否推荐
        public string Description { get; set; }
        public bool IsHotSale { get; set; }
        public long SaleNum { get; set; }
    }      
}
