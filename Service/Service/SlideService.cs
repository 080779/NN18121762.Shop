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
    public class SlideService : ISlideService
    {
        private SlideDTO ToDTO(SlideEntity entity)
        {
            SlideDTO dto = new SlideDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Code = entity.Code;
            dto.ImgUrl = entity.ImgUrl;
            dto.Name = entity.Name;
            dto.IsEnabled = entity.IsEnabled;
            dto.Url = entity.Url;
            return dto;
        }
        public async Task<long> AddAsync(string name, string url, string imgUrl, bool isEnabled)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SlideEntity entity = new SlideEntity();
                entity.Code = DateTime.Now.ToString("yyyyMMddHHmmss");
                entity.Name = name;
                entity.Url = url;
                entity.ImgUrl = imgUrl;
                entity.IsEnabled = isEnabled;
                dbc.Slides.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SlideEntity entity = await dbc.GetAll<SlideEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<SlideDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SlideEntity entity = await dbc.GetAll<SlideEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }               
                return ToDTO(entity);
            }
        }

        public async Task<SlideSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SlideSearchResult result = new SlideSearchResult();
                var entities = dbc.GetAll<SlideEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Name.Contains(keyword));
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
                var noticesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Slides = noticesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string name, string url, string imgUrl, bool isEnabled)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                SlideEntity entity = await dbc.GetAll<SlideEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Name = name;
                entity.Url = url;
                entity.ImgUrl = imgUrl;
                entity.IsEnabled = isEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
