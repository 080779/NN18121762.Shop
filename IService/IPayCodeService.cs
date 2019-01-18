using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 付款码管理接口
    /// </summary>
    public interface IPayCodeService : IServiceSupport
    {
        Task<long> AddAsync(long userId,string name, string codeUrl,string description);
        Task<bool> UpdateAsync(long id, string name, string codeUrl, string description);
        Task<bool> DeleteAsync(long id);
        Task<PayCodeDTO[]> GetModelByUserIdAsync(long id);
        Task<PayCodeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class PayCodeSearchResult
    {
        public PayCodeDTO[] BankAccounts { get; set; }
        public long PageCount { get; set; }
    }
}
