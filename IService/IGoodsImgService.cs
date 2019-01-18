using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 商品图片管理接口
    /// </summary>
    public interface IGoodsImgService : IServiceSupport
    {
        Task<long> AddAsync(long goodsId, string name,string imgUrl, string description);
        Task<long> AddAsync(long goodsId, List<string> imgUrls);
        Task<bool> UpdateAsync(long id, string name, string imgUrl, string description);
        Task<bool> DeleteAsync(long id);
        string GetFirstImg(long? goodsId);
        GoodsImgDTO[] GetModelList(long? goodsId);
        Task<GoodsImgDTO[]> GetModelListAsync(long? goodsId);
        Task<GoodsImgSearchResult> GetModelListAsync(long? goodsId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class GoodsImgSearchResult
    {
        public GoodsImgDTO[] GoodsImgs { get; set; }
        public long PageCount { get; set; }
    }
}
