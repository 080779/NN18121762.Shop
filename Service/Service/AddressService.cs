using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class AddressService : IAddressService
    {
        public AddressDTO ToDTO(AddressEntity entity)
        {
            AddressDTO dto = new AddressDTO();
            dto.Description = entity.Description;
            dto.Name = entity.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Address = entity.Address;
            dto.Mobile = entity.Mobile;
            dto.UserId = entity.UserId;
            dto.IsDefault = entity.IsDefault;
            return dto;
        }
        public async Task<long> AddAsync(long userId, string name, string mobile, string address,bool isDefault)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AddressEntity entity = new AddressEntity();
                entity.UserId = userId;
                entity.Name = name;
                entity.Mobile = mobile;
                entity.Address = address;
                if(isDefault)
                {
                    await dbc.GetAll<AddressEntity>().Where(a => a.UserId == userId).ForEachAsync(a => a.IsDefault = false);
                    entity.IsDefault = isDefault;
                }
                else
                {
                    entity.IsDefault = isDefault;
                }
                dbc.Addresses.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AddressEntity entity = await dbc.GetAll<AddressEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<AddressDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AddressEntity>().AsNoTracking().SingleOrDefaultAsync(a=>a.Id==id);
                if(entity==null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<AddressDTO> GetDefaultModelAsync(long userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<AddressEntity>().AsNoTracking().SingleOrDefaultAsync(a => a.UserId == userId && a.IsDefault==true);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<AddressSearchResult> GetModelListAsync(long? userId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AddressSearchResult result = new AddressSearchResult();
                var entities = dbc.GetAll<AddressEntity>().AsNoTracking();
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Name.Contains(keyword) || g.Description.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var addressResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Address = addressResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }       

        public async Task<bool> UpdateAsync(long id, string name, string mobile, string address, bool isDefault)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                AddressEntity entity = await dbc.GetAll<AddressEntity>().SingleOrDefaultAsync(a=>a.Id==id);
                if(entity==null)
                {
                    return false;
                }
                entity.Name = name;
                entity.Mobile = mobile;
                entity.Address = address;
                if (!entity.IsDefault && isDefault)
                {
                    await dbc.GetAll<AddressEntity>().Where(a => a.UserId == entity.UserId).ForEachAsync(a => a.IsDefault = false);
                    entity.IsDefault = isDefault;
                }
                else
                {
                    entity.IsDefault = isDefault;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
