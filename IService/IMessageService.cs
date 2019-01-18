using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 短信发送信息接口
    /// </summary>
    public interface IMessageService : IServiceSupport
    {
        Task<long> AddAsync(long? userId, string mobile, string content, int? flag);
    }
}
