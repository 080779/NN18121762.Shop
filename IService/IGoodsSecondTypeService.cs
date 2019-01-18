using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品分类管理接口
    /// </summary>
    public interface IGoodsSecondTypeService : IServiceSupport
    {
        Task<long> AddAsync(long goodsTypeId, string name, string description);
        Task<bool> UpdateAsync(long id, string name, string description);
        Task<bool> DeleteAsync(long id);
        Task<GoodsSecondTypeDTO> GetModelAsync(long id);
        Task<long> GetIdByNameAsync(string name);
        Task<GoodsSecondTypeSearchResult> GetModelListAsync(long? goodTypeId,string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class GoodsSecondTypeSearchResult
    {
        public GoodsSecondTypeDTO[] GoodsSecondTypes { get; set; }
        public long PageCount { get; set; }
    }   
}
