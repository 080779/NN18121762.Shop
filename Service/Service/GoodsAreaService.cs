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
    public class GoodsAreaService : IGoodsAreaService
    {
        private GoodsAreaDTO ToDTO(GoodsAreaEntity entity)
        {
            GoodsAreaDTO dto = new GoodsAreaDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Note = entity.Note;
            dto.Title = entity.Title;
            return dto;
        }
        public async Task<long> AddAsync(string title, string description, string note)
        {
            using (MyDbContext dbc=new MyDbContext())
            {
                GoodsAreaEntity entity = new GoodsAreaEntity();
                entity.Title = title;
                entity.Description = description;
                entity.Note = note;
                dbc.GoodsAreas.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsAreaEntity entity =await dbc.GetAll<GoodsAreaEntity>().SingleOrDefaultAsync(g=>g.Id==id);
                if(entity==null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> GetIdByTitleAsync(string title)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsAreaEntity entity = await dbc.GetAll<GoodsAreaEntity>().SingleOrDefaultAsync(g => g.Title == title);
                if (entity == null)
                {
                    return 0;
                }
                return entity.Id;
            }
        }

        public async Task<GoodsAreaDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsAreaEntity entity = await dbc.GetAll<GoodsAreaEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<GoodsAreaSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsAreaSearchResult result = new GoodsAreaSearchResult();
                var entities= dbc.GetAll<GoodsAreaEntity>().AsNoTracking();
                if(!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g=>g.Title.Contains(keyword) || g.Note.Contains(keyword) || g.Description.Contains(keyword));
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
                var goodsAreaResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.GoodsAreas = goodsAreaResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string title, string description, string note)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsAreaEntity entity = await dbc.GetAll<GoodsAreaEntity>().SingleOrDefaultAsync(g=>g.Id==id);
                if(entity==null)
                {
                    return false;
                }
                entity.Title = title;
                entity.Description = description;
                entity.Note = note;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
