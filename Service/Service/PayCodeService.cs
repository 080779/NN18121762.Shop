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
    public class PayCodeService : IPayCodeService
    {
        public PayCodeDTO ToDTO(PayCodeEntity entity)
        {
            PayCodeDTO dto = new PayCodeDTO();
            dto.Description = entity.Description;
            dto.Name = entity.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.CodeUrl = entity.CodeUrl;
            dto.UserId = entity.UserId;
            return dto;
        }

        public async Task<long> AddAsync(long userId, string name, string codeUrl, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                PayCodeEntity entity = new PayCodeEntity();
                entity.UserId = userId;
                entity.Name = name;
                entity.CodeUrl = codeUrl;
                entity.Description = description;
                dbc.PayCodes.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> UpdateAsync(long id, string name, string codeUrl, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                PayCodeEntity entity = await dbc.GetAll<PayCodeEntity>().SingleOrDefaultAsync(b=>b.Id==id); 
                if(entity==null)
                {
                    return false;
                }
                if(!string.IsNullOrEmpty(name))
                {
                    entity.Name = name;
                }
                if (!string.IsNullOrEmpty(codeUrl))
                {
                    entity.CodeUrl = codeUrl;

                }
                if (!string.IsNullOrEmpty(description))
                {
                    entity.Description = description;
                }                
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                PayCodeEntity entity = await dbc.GetAll<PayCodeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<PayCodeDTO[]> GetModelByUserIdAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<PayCodeEntity>().AsNoTracking().Where(b=>b.IsNull==false && b.UserId==id);
                var payCodes = await entities.ToListAsync();
                return payCodes.Select(p => ToDTO(p)).ToArray();
            }
        }

        public Task<PayCodeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
