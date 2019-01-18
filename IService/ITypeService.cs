using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface ITypeService : IServiceSupport
    {
        Task<T[]> GetModelListAsync<T>();
    }
}
