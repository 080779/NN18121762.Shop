using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 产品分区接口
    /// </summary>
    public interface IGoodsAreaService : IServiceSupport
    {
        Task<long> AddAsync(string title,string decription,string note);
        Task<bool> UpdateAsync(long id, string title, string decription, string note);
        Task<bool> DeleteAsync(long id);
        Task<long> GetIdByTitleAsync(string title);
        Task<GoodsAreaDTO> GetModelAsync(long id);
        Task<GoodsAreaSearchResult> GetModelListAsync(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class GoodsAreaSearchResult
    {
        public GoodsAreaDTO[] GoodsAreas { get; set; }
        public long PageCount { get; set; }
    }   
}
