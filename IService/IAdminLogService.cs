using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IAdminLogService:IServiceSupport
    {
        Task<long> AddAsync(long adminId,long permissionTypeId,string description,string ipAddress,string tip);
        long Add(long adminId,string permissionType,string description,string ipAddress,string tip);
        Task<AdminLogSearchResult> GetModelListAsync(string keyword, long? permissionTypeId, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class AdminLogSearchResult
    {
        public AdminLogDTO[] AdminLogs { get; set; }
        public long PageCount { get; set; }
    }
}
