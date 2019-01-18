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
    public class GoodsTypeService : IGoodsTypeService
    {
        private GoodsTypeDTO ToDTO(GoodsTypeEntity entity)
        {
            GoodsTypeDTO dto = new GoodsTypeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.ImgUrl = entity.ImgUrl;
            return dto;
        }
        public async Task<long> AddAsync(string name,string imgUrl, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsTypeEntity entity = new GoodsTypeEntity();
                entity.Description = description;
                entity.ImgUrl = imgUrl;
                entity.Name = name;
                dbc.GoodsTypes.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsTypeEntity entity = await dbc.GetAll<GoodsTypeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<GoodsTypeDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsTypeEntity entity = await dbc.GetAll<GoodsTypeEntity>().AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<GoodsTypeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsTypeSearchResult result = new GoodsTypeSearchResult();
                var entities = dbc.GetAll<GoodsTypeEntity>().AsNoTracking().Where(g=>g.IsNull == false);
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
                var goodsTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.GoodsTypes = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string name, string imgUrl, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsTypeEntity entity = await dbc.GetAll<GoodsTypeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Name = name;
                entity.Description = description;
                entity.ImgUrl = imgUrl;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
