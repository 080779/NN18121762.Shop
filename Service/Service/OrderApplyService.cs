using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class OrderApplyService : IOrderApplyService
    {
        private OrderApplyDTO ToDTO(OrderApplyEntity entity)
        {
            OrderApplyDTO dto = new OrderApplyDTO();
            dto.CreateTime = entity.CreateTime;
            dto.GoodsId = entity.GoodsId;
            dto.GoodsName = entity.GoodsName;
            dto.Id = entity.Id;
            dto.ImgUrl = entity.ImgUrl;
            dto.Number = entity.Number;
            dto.Price = entity.Price;
            dto.TotalFee = entity.TotalFee;
            dto.UserId = entity.UserId;
            return dto;
        }

        public async Task<long> AddAsync(params GoodsCarDTO[] goodsCars)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                foreach(GoodsCarDTO goodsCar in goodsCars)
                {
                    OrderApplyEntity entity = new OrderApplyEntity();
                    entity.GoodsId = goodsCar.GoodsId;
                    entity.GoodsName = goodsCar.Name;
                    entity.ImgUrl = goodsCar.ImgUrl;
                    entity.Number = goodsCar.Number;
                    entity.Price = goodsCar.RealityPrice;
                    entity.TotalFee = goodsCar.GoodsAmount;
                    entity.UserId = goodsCar.UserId;
                    dbc.OrderApplies.Add(entity);
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<bool> DeleteListAsync(long userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                await dbc.GetAll<OrderApplyEntity>().Where(o => o.UserId == userId).ForEachAsync(o => dbc.OrderApplies.Remove(o));
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<OrderApplySearchResult> GetModelListAsync(long userId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderApplySearchResult result = new OrderApplySearchResult();
                var entities = dbc.GetAll<OrderApplyEntity>().AsNoTracking().Where(o => o.UserId == userId);
                var res= await entities.ToListAsync();
                result.OrderApplies = res.Select(o => ToDTO(o)).ToArray();
                result.ToTalAmount = res.Sum(o => o.TotalFee);
                return result;
            }
        }
    }
}
