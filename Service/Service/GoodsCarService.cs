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
    public class GoodsCarService : IGoodsCarService
    {
        private GoodsCarDTO ToDTO(GoodsCarEntity entity, string imgUrl)
        {
            GoodsCarDTO dto = new GoodsCarDTO();
            dto.GoodsId = entity.GoodsId;
            dto.UserId = entity.UserId;
            dto.Code = entity.Goods.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Goods.Description;
            dto.Id = entity.Id;
            if (string.IsNullOrEmpty(imgUrl))
            {
                dto.ImgUrl = "";
            }
            else
            {
                dto.ImgUrl = imgUrl;
            }
            dto.Name = entity.Goods.Name;
            dto.Price = entity.Goods.Price;
            dto.RealityPrice = entity.Goods.RealityPrice;
            dto.Standard = entity.Goods.Standard;
            dto.Number = entity.Number;
            dto.IsSelected = entity.IsSelected;
            dto.GoodsAmount = entity.Goods.RealityPrice * entity.Number;
            dto.Inventory = entity.Goods.Inventory;
            return dto;
        }
        public async Task<long> AddAsync(long userId, long goodsId, long num)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var goodsCar = await dbc.GetAll<GoodsCarEntity>().SingleOrDefaultAsync(g => g.UserId == userId && g.GoodsId == goodsId);
                var goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == goodsId);
                if (goods == null)
                {
                    return -1;
                }
                if (goodsCar != null)
                {
                    if (num <= 0)
                    {
                        if (num != -1)
                        {
                            return -3;//购物车减商品数量,只能传-1
                        }
                        if (goodsCar.Number == 1)
                        {
                            return -4;//购物车减商品数量,购物车商品数量不能小于1
                        }
                        goodsCar.Number = goodsCar.Number + num;
                    }
                    else
                    {
                        if (goods.Inventory < goodsCar.Number + num)
                        {
                            return -2;
                        }
                        goodsCar.Number = goodsCar.Number + num;
                    }
                }
                else
                {
                    if (num <= 0)
                    {
                        return -5;//如果记录不存在，添加购物车的商品数量不能小于1
                    }
                    if (goods.Inventory < num)
                    {
                        return -2;
                    }
                    goodsCar = new GoodsCarEntity();
                    goodsCar.GoodsId = goodsId;
                    goodsCar.UserId = userId;
                    goodsCar.Number = num;
                    dbc.GoodsCars.Add(goodsCar);
                }
                await dbc.SaveChangesAsync();
                return goodsCar.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarEntity entity = await dbc.GetAll<GoodsCarEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteListAsync(long userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                await dbc.GetAll<GoodsCarEntity>().Where(g => g.UserId == userId && g.IsSelected == true).ForEachAsync(g => dbc.GoodsCars.Remove(g));
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<GoodsCarDTO[]> GetModelListAsync(long? userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<GoodsCarEntity>().AsNoTracking().Where(g => g.IsSelected == true);
                if (userId != null)
                {
                    entities = entities.Where(g => g.UserId == userId);
                }
                var goodsCarResult = await entities.ToListAsync();
                var imgUrls = dbc.GetAll<GoodsImgEntity>().AsNoTracking().Select(g => new { g.GoodsId, g.ImgUrl });
                return goodsCarResult.Select(a => ToDTO(a, imgUrls.Where(g => g.GoodsId == a.GoodsId).Select(g => g.ImgUrl).FirstOrDefault())).ToArray();
            }
        }

        public async Task<GoodsCarSearchResult> GetModelListAsync(long? userId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarSearchResult result = new GoodsCarSearchResult();
                var entities = dbc.GetAll<GoodsCarEntity>().AsNoTracking();
                if (userId != null)
                {
                    entities = entities.Where(g => g.UserId == userId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Goods.Code.Contains(keyword) || g.Goods.Name.Contains(keyword) || g.Goods.Description.Contains(keyword));
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
                var goodsAreaResult = await entities.Include(g => g.Goods).OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                var imgUrls = dbc.GetAll<GoodsImgEntity>().AsNoTracking().Select(g => new { g.GoodsId, g.ImgUrl });
                result.GoodsCars = goodsAreaResult.Select(a => ToDTO(a, imgUrls.Where(g => g.GoodsId == a.GoodsId).Select(g => g.ImgUrl).FirstOrDefault())).ToArray();
                result.TotalAmount = result.GoodsCars.Where(g => g.IsSelected == true).Sum(g => g.GoodsAmount);
                return result;
            }
        }

        public async Task<GoodsCarSearchResult> GetModelListAsync(long? userId, bool? isSelected, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarSearchResult result = new GoodsCarSearchResult();
                var entities = dbc.GetAll<GoodsCarEntity>().Include(g => g.Goods).AsNoTracking();
                if (userId != null)
                {
                    entities = entities.Where(g => g.UserId == userId);
                }
                if (isSelected != null)
                {
                    entities = entities.Where(g => g.IsSelected == isSelected);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Goods.Code.Contains(keyword) || g.Goods.Name.Contains(keyword) || g.Goods.Description.Contains(keyword));
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
                var imgUrls = dbc.GetAll<GoodsImgEntity>().AsNoTracking().Select(g=>new { g.GoodsId,g.ImgUrl});
                result.GoodsCars = goodsAreaResult.Select(a => ToDTO(a, imgUrls.Where(g => g.GoodsId == a.GoodsId).Select(g => g.ImgUrl).FirstOrDefault())).ToArray();
                result.TotalAmount = result.GoodsCars.Where(g => g.IsSelected == true).Sum(g => g.GoodsAmount);
                return result;
            }
        }

        public async Task<long> UpdateAsync(long id, long? num, bool? isSelected)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarEntity entity = await dbc.GetAll<GoodsCarEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == entity.GoodsId);
                if (goods == null)
                {
                    return -2;
                }
                if (num != null)
                {
                    if (goods.Inventory < num)
                    {
                        return -3;
                    }
                    entity.Number = num.Value;
                }
                if (isSelected != null)
                {
                    entity.IsSelected = isSelected.Value;
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }
        public async Task<long> UpdateAsync(long userId, long goodsId, long? num, bool? isSelected)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsCarEntity entity = await dbc.GetAll<GoodsCarEntity>().SingleOrDefaultAsync(g => g.UserId == userId && g.GoodsId == goodsId);
                if (entity == null)
                {
                    return -1;
                }
                GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == entity.GoodsId);
                if (goods == null)
                {
                    return -2;
                }
                if (num != null)
                {
                    if (goods.Inventory < num)
                    {
                        return -3;
                    }
                    entity.Number = num.Value;
                }
                if (isSelected != null)
                {
                    entity.IsSelected = isSelected.Value;
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }
    }
}
