using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IAdminService : IServiceSupport
    {
        Task<long> AddAsync(string adminMobile, string mobile, string description, string password);
        Task<bool> UpdateAsync(long id, string mobile, string description, string password, long[] permissionIds);
        Task<bool> UpdateAsync(long id, long[] permissionIds);
        Task<bool> UpdateAsync(long id, string password);
        Task<bool> DeleteAsync(long id);
        Task<bool> FrozenAsync(long id);
        Task<AdminDTO> GetModelAsync(long id);
        Task<string> GetMobileAsync(long id);
        Task<AdminSearchResult> GetModelListAsync(string isAdmin, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<AdminSearchResult> GetModelListHasPerAsync(string isAdmin, string mobile, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        bool HasPermission(long id, string description);
        Task<long> CheckLogin(string mobile, string password);
        Task<bool> DelAll();
    }
    public class AdminSearchResult
    {
        public AdminDTO[] Admins { get; set; }
        public long PageCount { get; set; }
    }
}
