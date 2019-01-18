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
    public class OrderListService : IOrderListService
    {
        private OrderListDTO ToDTO(OrderListEntity entity)
        {
            OrderListDTO dto = new OrderListDTO();
            dto.CreateTime = entity.CreateTime;
            dto.GoodsId = entity.GoodsId;
            dto.GoodsName = entity.Goods.Name;
            dto.Id = entity.Id;
            dto.ImgUrl = entity.ImgUrl;
            dto.Number = entity.Number;
            dto.OrderCode = entity.Order.Code;
            dto.OrderId = entity.OrderId;
            dto.Price = entity.Goods.Price;
            dto.RealityPrice = entity.Goods.RealityPrice;
            dto.TotalFee = entity.TotalFee;
            dto.GoodsCode = entity.Goods.Code;
            dto.IsReturn = entity.IsReturn;
            dto.Inventory = entity.Goods.Inventory;
            dto.Discount = entity.Order.UpAmount==null?1: entity.Order.UpAmount.Value;
            dto.DiscountFee = dto.TotalFee * dto.Discount;
            return dto;
        }
        public async Task<long> AddAsync(long orderId, long goodsId, long number)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == goodsId);
                if(goods==null)
                {
                    return -1;
                }
                OrderListEntity entity = new OrderListEntity();
                entity.OrderId = orderId;
                entity.GoodsId = goodsId;
                entity.Number = number;
                entity.Price = goods.RealityPrice;
                string imgUrl = await dbc.GetAll<GoodsImgEntity>().Where(g => g.GoodsId == goodsId).Select(g=>g.ImgUrl).FirstOrDefaultAsync();
                if(imgUrl == null)
                {
                    entity.ImgUrl = "";
                }
                else
                {
                    entity.ImgUrl = imgUrl;
                }                
                entity.TotalFee = entity.Price * number;
                dbc.OrderLists.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> AddListAsync(List<OrderListAdd> goodsLists)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                foreach (var goods in goodsLists)
                {
                    GoodsEntity goodsEntity = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == goods.GoodsId);
                    if (goodsEntity == null)
                    {
                        return -1;
                    }
                    OrderListEntity entity = new OrderListEntity();
                    entity.OrderId = goods.OrderId;
                    entity.GoodsId = goods.GoodsId;
                    entity.Number = goods.Number;
                    entity.Price = goodsEntity.RealityPrice;
                    string imgUrl = await dbc.GetAll<GoodsImgEntity>().Where(g => g.GoodsId == goods.GoodsId).Select(g => g.ImgUrl).FirstOrDefaultAsync();
                    if (imgUrl == null)
                    {
                        entity.ImgUrl = "";
                    }
                    else
                    {
                        entity.ImgUrl = imgUrl;
                    }
                    entity.TotalFee = entity.Price * entity.Number;
                    dbc.OrderLists.Add(entity);
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListEntity entity = await dbc.GetAll<OrderListEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public OrderListDTO[] GetModelList(long? orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListSearchResult result = new OrderListSearchResult();
                var entities = dbc.GetAll<OrderListEntity>();
                if (orderId != null)
                {
                    entities = entities.Where(a => a.OrderId == orderId);
                }
                return entities.ToList().Select(o => ToDTO(o)).ToArray();
            }
        }

        public async Task<OrderListSearchResult> GetModelListAsync(long? orderId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListSearchResult result = new OrderListSearchResult();
                var entities = dbc.GetAll<OrderListEntity>().Include(o=>o.Goods).Include(o=>o.Order).AsNoTracking();
                if (orderId != null)
                {
                    entities = entities.Where(a => a.OrderId == orderId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Goods.Name.Contains(keyword) || g.Goods.Code.Contains(keyword));
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
                var orderListResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.OrderLists = orderListResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> SetDiscountAmountAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var lists = await dbc.GetAll<OrderListEntity>().ToListAsync(); ;
                foreach(var list in lists)
                {
                    OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o=>o.Id==list.OrderId);
                    decimal a = order == null ? 1 : order.UpAmount.Value;
                    list.DiscountFee = list.TotalFee * a;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateAsync(long id, long number)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListEntity entity = await dbc.GetAll<OrderListEntity>().SingleOrDefaultAsync(o => o.Id == id);
                if(entity==null)
                {
                    return false;
                }
                entity.Number = number;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> SetIsReturnAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderListEntity entity = await dbc.GetAll<OrderListEntity>().SingleOrDefaultAsync(o => o.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsReturn = !entity.IsReturn;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ReSetIsReturnAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                await dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == orderId).ForEachAsync(g=>g.IsReturn=false);                
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
