using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 收货地址管理接口
    /// </summary>
    public interface IAddressService : IServiceSupport
    {
        Task<long> AddAsync(long userId, string name, string mobile,string sheng,string shi,string qu, string address, bool isDefault);
        Task<bool> UpdateAsync(long id, string name, string mobile, string sheng, string shi, string qu, string address, bool isDefault);
        Task<bool> DeleteAsync(long id);
        Task<AddressDTO> GetModelAsync(long id);
        Task<AddressDTO> GetDefaultModelAsync(long userId);
        Task<AddressSearchResult> GetModelListAsync(long? userId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);
    }
    public class AddressSearchResult
    {
        public AddressDTO[] Address { get; set; }
        public long PageCount { get; set; }
    }
}
