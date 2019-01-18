using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品分类管理接口
    /// </summary>
    public interface IGoodsTypeService : IServiceSupport
    {
        Task<long> AddAsync(string name, string imgUrl, string description);
        Task<bool> UpdateAsync(long id, string name, string imgUrl, string description);
        Task<bool> DeleteAsync(long id);
        Task<GoodsTypeDTO> GetModelAsync(long id);
        Task<GoodsTypeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class GoodsTypeSearchResult
    {
        public GoodsTypeDTO[] GoodsTypes { get; set; }
        public long PageCount { get; set; }
    }
}
