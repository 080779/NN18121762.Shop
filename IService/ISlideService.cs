using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 幻灯片管理接口
    /// </summary>
    public interface ISlideService : IServiceSupport
    {
        Task<long> AddAsync(string name,string url,string imgUrl,bool isEnabled);
        Task<bool> UpdateAsync(long id, string name, string url, string imgUrl, bool isEnabled);
        Task<bool> DeleteAsync(long id);
        Task<SlideDTO> GetModelAsync(long id);
        Task<SlideSearchResult> GetModelListAsync(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class SlideSearchResult
    {
        public SlideDTO[] Slides { get; set; }
        public long PageCount { get; set; }
    }   
}
