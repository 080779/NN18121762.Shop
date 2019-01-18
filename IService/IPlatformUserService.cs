using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IPlatformUserService:IServiceSupport
    {
        /// <summary>
        /// 根据平台用户类型添加平台用户
        /// </summary>
        /// <param name="typeName">平台用户类型名</param>
        /// <param name="mobile">账号</param>
        /// <param name="code">编号</param>
        /// <param name="password">密码</param>
        /// <param name="tradePassword">交易密码</param>
        /// <returns></returns>
        Task<long> AddAsync(long id, string typeName,string mobile,string code,string password,string tradePassword);
        Task<bool> DelAsync(long id);
        Task<bool> UpdatePwdAsync(long id,string password);
        Task<bool> UpdateTradePwdAsync(long id, string tradePassword);
        Task<PlatformUserDTO> GetModelAsync(long id);
        Task<PlatformUserDTO> GetModelAsync(string type, string str);
        Task<PlatformUserSearchResult> GetModelListAsync(string mobile, string code, string type, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
        Task<bool> Frozen(long id);
        Task<long> CheckLoginAsync(string mobile,string password);
        Task<bool> CheckTradePasswordAsync(long id, string password);
        Task<bool> IsExist(string type, string str);
        Task<bool> ProvideAsync(long userId, long toUserId, long Integral, string typeName, string toTypeName, string description,string tip);
        Task<bool> TakeOutAsync(long userId, long takeOutUserid, long integral, string typeName, string description,string tip);
        Task<bool> TakeCashApplyAsync(long userId,long integral,string typeName,string description);
        Task<int> TakeCashConfirmAsync(long id);
        Task<bool> TakeCashCancelAsync(long id);
    }
    public class PlatformUserSearchResult
    {
        public PlatformUserDTO[] PlatformUsers { get; set; }
        public long TotalCount { get; set; }
    }
}
