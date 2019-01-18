using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 奖金接口
    /// </summary>
    public interface IBonusService : IServiceSupport
    {
        Task<long> AddAsync(long userId, decimal amount, decimal revenue, decimal sf, string source, long fromUserID, int isSettled);        
        Task<bool> DeleteAsync(long id);
        Task<BonusDTO> GetModelAsync(long id);
        Task<BonusSearchResult> GetModelListAsync(long? userId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<BonusSearchResult> BoussListAsync(long? userId, long? typeId, int pageIndex, int pageSize);
        Task<bool> CalcShareBonusAsync(decimal totalScore);
    }
    public class BonusSearchResult
    {
        public BonusDTO[] List { get; set; }
        public long PageCount { get; set; }
    }

}
