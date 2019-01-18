using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class UserTokenService : IUserTokenService
    {
        public async Task<long> AddAsync(long userId, string token)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTokenEntity userToken = await dbc.GetAll<UserTokenEntity>().SingleOrDefaultAsync(u => u.UserId == userId);
                if (userToken != null)
                {
                    return -1;
                }
                UserTokenEntity Token = new UserTokenEntity();
                Token.UserId = userId;
                Token.Token = token;
                dbc.UserTokens.Add(Token);
                await dbc.SaveChangesAsync();
                return Token.Id;
            }
        }

        public async Task<long> UpdateAsync(long userId, string token)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTokenEntity userToken = await dbc.GetAll<UserTokenEntity>().SingleOrDefaultAsync(u => u.UserId == userId);
                if (userToken != null)
                {
                    userToken.Token = token;
                }
                else
                {
                    UserTokenEntity entity = new UserTokenEntity();
                    entity.UserId = userId;
                    entity.Token = token;
                    dbc.UserTokens.Add(entity);
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public long CheckToken(long userId, string token)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var userToken= dbc.GetAll<UserTokenEntity>().SingleOrDefault(u=>u.UserId==userId);
                if(userToken==null)
                {
                    return -1;
                }
                if(string.IsNullOrEmpty(userToken.Token))
                {
                    return -2;
                }
                if(userToken.Token!=token)
                {
                    return -3;
                }
                return 1;
            }
        }
    }
}
