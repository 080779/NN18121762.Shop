using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IIdNameService : IServiceSupport
    {
        Task<long> GetIdByNameAsync(string name);
        Task<IdNameDTO> GetByNameAsync(string name);
        Task<IdNameDTO[]> GetByTypeNameAsync(string typeName);
    }
}
