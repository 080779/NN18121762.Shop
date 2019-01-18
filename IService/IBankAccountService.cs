using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 银行卡管理接口
    /// </summary>
    public interface IBankAccountService : IServiceSupport
    {
        Task<long> AddAsync(long userId, string name, string bankAccount, string bankName);
        Task<bool> UpdateAsync(long id, string name, string bankAccount, string bankName);
        Task<bool> UpdateByUserIdAsync(long id, string name, string bankAccount, string bankName);
        Task<bool> DeleteAsync(long id);
        Task<BankAccountDTO[]> GetModelListByUserIdAsync(long id);
        Task<BankAccountDTO> GetModelAsync(long id);
        Task<BankAccountDTO> GetModelByUserIdAsync(long id);
        Task<BankAccountSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class BankAccountSearchResult
    {
        public BankAccountDTO[] BankAccounts { get; set; }
        public long PageCount { get; set; }
    }
}
