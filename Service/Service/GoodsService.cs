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
    public class GoodsService : IGoodsService
    {
        private GoodsDTO ToDTO(GoodsEntity entity)
        {
            GoodsDTO dto = new GoodsDTO();
            dto.Code = entity.Code;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.GoodsAreaId = entity.GoodsAreaId;
            dto.GoodsAreaTitle = "";
            dto.GoodsSecondTypeId = entity.GoodsSecondType.Name == "空类型" ? (long?)null : entity.GoodsSecondTypeId;
            dto.GoodsSecondTypeName = entity.GoodsSecondType.Name == "空类型" ? "" : entity.GoodsSecondType.Name;
            dto.GoodsTypeId = entity.GoodsTypeId;
            dto.GoodsTypeName = entity.GoodsType.Name;
            dto.Id = entity.Id;
            dto.Inventory = entity.Inventory;
            dto.IsPutaway = entity.IsPutaway;
            dto.IsRecommend = entity.IsRecommend;
            dto.Name = entity.Name;
            dto.Price = entity.Price;
            dto.RealityPrice = entity.RealityPrice;
            dto.SaleNum = entity.SaleNum;
            dto.Standard = entity.Standard;
            dto.IsHotSale = entity.IsHotSale;
            return dto;
        }
        public async Task<long> AddAsync(GoodsAddEditModel goods)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity entity = new GoodsEntity();
                entity.Code = CommonHelper.GetRandom2();
                entity.Description = goods.Description;
                entity.GoodsAreaId = goods.GoodsAreaId;
                entity.GoodsSecondTypeId = goods.GoodsSecondTypeId.Value;
                entity.GoodsTypeId = goods.GoodsTypeId;
                entity.Inventory = goods.Inventory;
                entity.IsPutaway = goods.IsPutaway;
                entity.IsRecommend = goods.IsRecommend;
                entity.Name = goods.Name;
                entity.Price = goods.Price;
                entity.RealityPrice = goods.RealityPrice;
                entity.Standard = "件";
                entity.IsHotSale = goods.IsHotSale;
                entity.SaleNum = goods.SaleNum;
                dbc.Goods.Add(entity);
                await dbc.SaveChangesAsync();
                BonusRatioEntity bonusRatio = new BonusRatioEntity();
                bonusRatio.GoodsId = entity.Id;
                dbc.BonusRatios.Add(bonusRatio);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity entity = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<GoodsDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity entity = await dbc.GetAll<GoodsEntity>().Include(g => g.GoodsSecondType).Include(g => g.GoodsType).AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }
        
        public async Task<GoodsSearchResult> FrontModelListAsync(bool? isHotSale, long? goodsTypeId, long? goodsSecondTypeId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSearchResult result = new GoodsSearchResult();
                var entities = dbc.GetAll<GoodsEntity>().Include(g => g.GoodsSecondType).Include(g => g.GoodsType).AsNoTracking().Where(g=>g.IsPutaway==true);
                if (isHotSale != null)
                {
                    entities = entities.Where(a => a.IsHotSale == isHotSale);
                }
                if (goodsTypeId != null)
                {
                    entities = entities.Where(a => a.GoodsTypeId == goodsTypeId);
                }
                if (goodsSecondTypeId != null)
                {
                    entities = entities.Where(a => a.GoodsSecondTypeId == goodsSecondTypeId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g =>g.Name.Contains(keyword) || g.Description.Contains(keyword));
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
                result.Goods = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<GoodsSearchResult> GetModelListAsync(bool? isPutaway, long? goodsTypeId,long? goodsSecondTypeId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSearchResult result = new GoodsSearchResult();
                var entities = dbc.GetAll<GoodsEntity>().Include(g => g.GoodsSecondType).Include(g => g.GoodsType).AsNoTracking();
                if(isPutaway != null)
                {
                    entities = entities.Where(a => a.IsPutaway == isPutaway);
                }
                if(goodsTypeId!=null)
                {
                    entities = entities.Where(a => a.GoodsTypeId == goodsTypeId);
                }
                if (goodsSecondTypeId != null)
                {
                    entities = entities.Where(a => a.GoodsSecondTypeId == goodsSecondTypeId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g =>g.Name.Contains(keyword) || g.Description.Contains(keyword));
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
                result.Goods = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<GoodsSearchResult> SearchAsync(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsSearchResult result = new GoodsSearchResult();
                var entities = dbc.GetAll<GoodsEntity>().Include(g => g.GoodsSecondType).Include(g => g.GoodsType).AsNoTracking().Where(g=>g.IsPutaway==true);
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Name.Contains(keyword) || g.Description.Contains(keyword) || g.GoodsSecondType.Name.Contains(keyword) || g.GoodsType.Name.Contains(keyword));
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
                result.Goods = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(GoodsAddEditModel goods)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity entity = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g=>g.Id==goods.Id);                
                if (entity==null)
                {
                    return false;
                }
                entity.Description = goods.Description;
                entity.GoodsAreaId = goods.GoodsAreaId;
                entity.GoodsSecondTypeId = goods.GoodsSecondTypeId.Value;
                entity.GoodsTypeId = goods.GoodsTypeId;
                entity.Inventory = goods.Inventory;
                entity.IsPutaway = goods.IsPutaway;
                entity.IsRecommend = goods.IsRecommend;
                entity.Name = goods.Name;
                entity.Price = goods.Price;
                entity.RealityPrice = goods.RealityPrice;
                entity.IsHotSale = goods.IsHotSale;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
