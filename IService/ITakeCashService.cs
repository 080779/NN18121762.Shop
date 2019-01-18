using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface ITakeCashService:IServiceSupport
    {
        Task<long> AddAsync(long userId,int payTypeId,decimal amount,decimal poundage, string descripton);
        Task<long> ConfirmAsync(long id,long adminId);
        Task<long> CancelAsync(long id, string description, long adminId);
        Task<TakeCashSearchResult> GetModelListAsync(long? userId,long? stateId, string keyword, DateTime? startTime,DateTime? endTime,int pageIndex,int pageSize);
    }
    public class TakeCashSearchResult
    {
        public TakeCashDTO[] TakeCashes { get; set; }
        public long PageCount { get; set; }
    }
}
