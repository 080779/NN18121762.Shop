using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.IService
{
    public interface IUserTokenService : IServiceSupport
    {
        Task<long> AddAsync(long userId,string token);
        Task<long> UpdateAsync(long userId, string token);
        long CheckToken(long userId,string token);
    }
}
