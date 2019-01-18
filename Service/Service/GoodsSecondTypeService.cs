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
    public class GoodsSecondTypeService : IGoodsSecondTypeService
    {
        private GoodsSecondTypeDTO ToDTO(GoodsSecondTypeEntity entity)
        {
            GoodsSecondTypeDTO dto = new GoodsSecondTypeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            return dto;
        }
        public async Task<long> AddAsync(long goodsTypeId,string name, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSecondTypeEntity entity = new GoodsSecondTypeEntity();
                entity.Description = description;
                entity.Name = name;
                entity.GoodsTypeId = goodsTypeId;
                dbc.GoodsSecondTypes.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSecondTypeEntity entity = await dbc.GetAll<GoodsSecondTypeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<GoodsSecondTypeDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSecondTypeEntity entity = await dbc.GetAll<GoodsSecondTypeEntity>().AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<long> GetIdByNameAsync(string name)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                return await dbc.GetEntityIdAsync<GoodsSecondTypeEntity>(g => g.Name == name);
            }
        }

        public async Task<GoodsSecondTypeSearchResult> GetModelListAsync(long? goodTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSecondTypeSearchResult result = new GoodsSecondTypeSearchResult();
                var entities = dbc.GetAll<GoodsSecondTypeEntity>().AsNoTracking().Where(g => g.IsNull == false);
                if (goodTypeId != null)
                {
                    entities = entities.Where(a => a.GoodsTypeId == goodTypeId);
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
                var goodsSecondTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.GoodsSecondTypes = goodsSecondTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string name, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSecondTypeEntity entity = await dbc.GetAll<GoodsSecondTypeEntity>().AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Name = name;
                entity.Description = description;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
