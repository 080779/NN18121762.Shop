using IMS.DTO;
using System;
using System.Threading.Tasks;

namespace IMS.IService
{
    /// <summary>
    /// 设置管理接口
    /// </summary>
    public interface ISettingService : IServiceSupport
    {
        Task<bool> EditAsync(long id, string parameter);
        Task<bool> EditAsync(params SettingSetDTO[] settings);
        Task<string> GetParmByNameAsync(string name);
        Task<SettingDTO> GetModelAsync(long id);
        Task<SettingDTO[]> GetModelListIsEnableAsync();
    }
    public class SettingSearchResult
    {
        public SettingDTO[] Settings { get; set; }
        public long PageCount { get; set; }
    }
    public class SettingParm
    {
        public long Id { get; set; }
        public string Parm { get; set; }
    }
}
