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
    public class NoticeService : INoticeService
    {
        private NoticeDTO ToDTO(NoticeEntity entity)
        {
            NoticeDTO dto = new NoticeDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Code = entity.Code;
            dto.Content = entity.Content;
            dto.FailureTime = entity.FailureTime;
            dto.IsEnabled = entity.IsEnabled;
            dto.Url = entity.Url;
            return dto;
        }
        public async Task<long> AddAsync(string code,string content, DateTime failureTime)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeEntity entity = new NoticeEntity();
                entity.Code = code;
                entity.Content = content;
                entity.Url = "";
                entity.FailureTime = failureTime;
                if(entity.FailureTime>DateTime.Now)
                {
                    entity.IsEnabled = true;
                }
                else
                {
                    entity.IsEnabled = false;
                }
                dbc.Notices.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeEntity entity = await dbc.GetAll<NoticeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<NoticeDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeEntity entity = await dbc.GetAll<NoticeEntity>().AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<NoticeSearchResult> GetModelListAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeSearchResult result = new NoticeSearchResult();
                await dbc.GetAll<NoticeEntity>().ForEachAsync(n => { if(n.FailureTime<DateTime.Now){ n.IsEnabled = false;}});
                await dbc.SaveChangesAsync();
                var entities = dbc.GetAll<NoticeEntity>().AsNoTracking();
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Content.Contains(keyword));
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
                result.Notices = noticesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, string code, string content, DateTime failureTime)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                NoticeEntity entity = await dbc.GetAll<NoticeEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.Code = code;
                entity.Content = content;
                entity.FailureTime = failureTime;
                if (entity.FailureTime > DateTime.Now)
                {
                    entity.IsEnabled = true;
                }
                else
                {
                    entity.IsEnabled = false;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
