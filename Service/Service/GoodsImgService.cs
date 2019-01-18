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
    public class GoodsImgService : IGoodsImgService
    {
        private GoodsImgDTO ToDTO(GoodsImgEntity entity)
        {
            GoodsImgDTO dto = new GoodsImgDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.Name = entity.Name;
            dto.ImgUrl = entity.ImgUrl;
            return dto;
        }
        public async Task<long> AddAsync(long goodsId,string name, string imgUrl, string description)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsImgEntity entity = new GoodsImgEntity();
                entity.Name = name;
                entity.GoodsId = goodsId;
                entity.ImgUrl = imgUrl;
                entity.Description = description;
                dbc.GoodsImgs.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> AddAsync(long goodsId, List<string> imgUrls)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                await dbc.GetAll<GoodsImgEntity>().Where(g => g.GoodsId == goodsId).ForEachAsync(g => g.IsDeleted = true);
                foreach(string imgUrl in imgUrls)
                {
                    GoodsImgEntity entity = new GoodsImgEntity();
                    entity.Name = "";
                    entity.GoodsId = goodsId;
                    entity.ImgUrl = imgUrl;
                    entity.Description = "";
                    dbc.GoodsImgs.Add(entity);            
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public Task<bool> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public string GetFirstImg(long? goodsId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string imgUrl = dbc.GetAll<GoodsImgEntity>().Where(g=>g.GoodsId==goodsId).Select(g => g.ImgUrl).FirstOrDefault();
                if(imgUrl==null)
                {
                    return "";
                }
                return imgUrl;
            }
        }

        public GoodsImgDTO[] GetModelList(long? goodsId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<GoodsImgEntity>().AsNoTracking();
                if (goodsId != null)
                {
                    entities = entities.Where(a => a.GoodsId == goodsId);
                }
                return entities.Select(g => ToDTO(g)).ToArray();
            }
        }

        public async Task<GoodsImgDTO[]> GetModelListAsync(long? goodsId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<GoodsImgEntity>().AsNoTracking();
                if (goodsId != null)
                {
                    entities = entities.Where(a => a.GoodsId == goodsId);
                }
                var res= await entities.ToListAsync();
                return res.Select(g => ToDTO(g)).ToArray();
            }
        }

        public async Task<GoodsImgSearchResult> GetModelListAsync(long? goodsId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsImgSearchResult result = new GoodsImgSearchResult();
                var entities = dbc.GetAll<GoodsImgEntity>().AsNoTracking();
                if(goodsId!=null)
                {
                    entities = entities.Where(a => a.GoodsId == goodsId);
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
                var goodsImgsResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.GoodsImgs = goodsImgsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public Task<bool> UpdateAsync(long id, string name, string imgUrl, string description)
        {
            throw new NotImplementedException();
        }
    }
}
