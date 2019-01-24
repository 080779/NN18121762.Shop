using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Service.Entity;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class OrderService : IOrderService
    {
        private static ILog log = LogManager.GetLogger(typeof(OrderService));
        private OrderDTO ToDTO(OrderEntity entity)
        {
            OrderDTO dto = new OrderDTO();
            dto.CreateTime = entity.CreateTime;
            dto.Amount = entity.Amount;
            dto.BuyerId = entity.BuyerId;
            dto.BuyerNickName = entity.Buyer.NickName;
            dto.BuyerMobile = entity.Buyer.Mobile;
            dto.Code = entity.Code;
            dto.Id = entity.Id;
            dto.OrderStateId = entity.OrderStateId;
            dto.OrderStateName = entity.OrderStateId.GetEnumName<OrderStateEnum>();
            dto.PayTypeId = entity.PayTypeId;
            dto.PayTypeName = entity.PayTypeId.GetEnumName<PayTypeEnum>();
            dto.ReceiverAddress = string.IsNullOrEmpty(entity.ReceiverAddress) ? entity.Address.Address : entity.ReceiverAddress;
            dto.ReceiverMobile = string.IsNullOrEmpty(entity.ReceiverMobile) ? entity.Address.Mobile : entity.ReceiverMobile;
            dto.ReceiverName = string.IsNullOrEmpty(entity.ReceiverName) ? entity.Address.Name : entity.ReceiverName;
            dto.Deliver = entity.Deliver;
            dto.DeliverName = entity.DeliverName;
            dto.DeliverCode = entity.DeliverCode;
            dto.PostFee = entity.PostFee;
            dto.ApplyTime = entity.ApplyTime;
            dto.CloseTime = entity.CloseTime;
            dto.ConsignTime = entity.ConsignTime;
            dto.DeductAmount = entity.DeductAmount;
            dto.EndTime = entity.EndTime;
            dto.IsRated = entity.IsRated;
            dto.PayTime = entity.PayTime;
            dto.RefundAmount = entity.RefundAmount;
            dto.AuditStatusId = entity.AuditStatusId;
            dto.DownCycledId = entity.DownCycledId;
            dto.AuditTime = entity.AuditTime;
            dto.AuditMobile = entity.AuditMobile;
            dto.ReturnAmount = entity.ReturnAmount;
            dto.DiscountAmount = entity.DiscountAmount;
            dto.UpAmount = entity.UpAmount;
            dto.UserDeliverCode = entity.UserDeliverCode;
            dto.UserDeliverName = entity.UserDeliverName;
            return dto;
        }

        public async Task<long> AddAsync(long buyerId, long addressId, int payTypeId, int orderStateId, long goodsId, long number)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = new OrderEntity();
                entity.Code = CommonHelper.GetRandom3();
                entity.BuyerId = buyerId;
                entity.AddressId = addressId;
                entity.PayTypeId = payTypeId;
                entity.OrderStateId = orderStateId;
                dbc.Orders.Add(entity);
                await dbc.SaveChangesAsync();

                GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == goodsId);
                if (goods == null)
                {
                    return -1;
                }
                OrderListEntity listEntity = new OrderListEntity();
                listEntity.OrderId = entity.Id;
                listEntity.GoodsId = goodsId;
                listEntity.Number = number;
                listEntity.Price = goods.RealityPrice;
                GoodsImgEntity imgEntity = await dbc.GetAll<GoodsImgEntity>().FirstOrDefaultAsync(g => g.GoodsId == goodsId);
                if (imgEntity == null)
                {
                    listEntity.ImgUrl = "";
                }
                else
                {
                    listEntity.ImgUrl = imgEntity.ImgUrl;
                }
                listEntity.TotalFee = listEntity.Price * number;
                entity.Amount = listEntity.TotalFee;
                dbc.OrderLists.Add(listEntity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> AddAsync(long? deliveryTypeId,decimal? postFee,long buyerId, long addressId, int payTypeId, int orderStateId, params OrderApplyDTO[] orderApplies)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        OrderEntity entity = new OrderEntity();
                        entity.Code = CommonHelper.GetRandom3();
                        entity.BuyerId = buyerId;
                        entity.AddressId = addressId;
                        entity.PayTypeId = payTypeId;
                        entity.OrderStateId = orderStateId;
                        entity.PostFee = postFee.Value;
                        //if(deliveryTypeId==1)
                        //{
                        //    entity.Deliver = "有快递单号";
                        //}
                        //if (deliveryTypeId == 2)
                        //{
                        //    entity.Deliver = "无需物流";
                        //}
                        //if (deliveryTypeId == 3)
                        //{
                        //    entity.Deliver = "同城自取";
                        //}

                        UserEntity user = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Id == buyerId);
                        if (user == null)
                        {
                            scope.Rollback();
                            return -2;
                        }
                        if(!user.IsEnabled)
                        {
                            return -6;
                        }

                        decimal discount;

                        string discountstr = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == "复购会员优惠", s => s.Param);

                        decimal.TryParse(discountstr, out discount);
                        discount = discount == 0 ? 1 : discount;

                        if (user.LevelId > (int)LevelEnum.用户)
                        {
                            entity.UpAmount = ((discount * 10) / 100);
                        }

                        dbc.Orders.Add(entity);
                        await dbc.SaveChangesAsync();

                        foreach (var orderApply in orderApplies)
                        {
                            GoodsEntity goods = await dbc.GetAll<GoodsEntity>().AsNoTracking().SingleOrDefaultAsync(g => g.Id == orderApply.GoodsId);
                            if (goods == null)
                            {
                                scope.Rollback();
                                return -1;
                            }
                            if (!goods.IsPutaway)
                            {
                                scope.Rollback();
                                return -4;
                            }
                            OrderListEntity listEntity = new OrderListEntity();
                            listEntity.OrderId = entity.Id;
                            listEntity.GoodsId = goods.Id;
                            listEntity.Number = orderApply.Number;
                            listEntity.Price = goods.RealityPrice;
                            listEntity.ImgUrl = orderApply.ImgUrl;
                            listEntity.TotalFee = listEntity.Price * listEntity.Number;
                            entity.Amount = entity.Amount + listEntity.TotalFee;
                            listEntity.DiscountFee = listEntity.TotalFee * entity.UpAmount.Value;
                            dbc.OrderLists.Add(listEntity);
                        }
                        //如果
                        if (entity.Amount == (decimal)0.01)
                        {
                            entity.DiscountAmount = (decimal)0.01;
                        }
                        else
                        {
                            entity.DiscountAmount = entity.Amount * entity.UpAmount.Value;
                        }
                        entity.Amount = entity.DiscountAmount + entity.PostFee;
                        await dbc.SaveChangesAsync();
                        scope.Commit();
                        return entity.Id;
                    }
                    catch (Exception ex)
                    {
                        scope.Rollback();
                        return -3;
                    }
                }                       
            }
        }

        public async Task<bool> AddUserDeliverAsync(long orderId, string userDeliverCode, string userDeliverName)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == orderId);
                if (entity == null)
                {
                    return false;
                }
                entity.UserDeliverCode = userDeliverCode;
                entity.UserDeliverName = userDeliverName;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> FrontMarkDel(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsRated = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<OrderDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().Include(o => o.Address).SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<OrderDTO[]> GetAllAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = await dbc.GetAll<OrderEntity>().Include(o => o.Address).OrderByDescending(a => a.CreateTime).ToListAsync();

                return entities.Select(o => ToDTO(o)).ToArray();
            }
        }

        public async Task<OrderSearchResult> GetModelListAsync(long? buyerId, int? orderStateId,long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                var entities = dbc.GetAll<OrderEntity>().Include(o => o.Address);
                if(buyerId!=null)
                {
                    entities = entities.Where(a => a.BuyerId ==buyerId);
                }
                if (orderStateId != null)
                {
                    entities = entities.Where(a => a.OrderStateId ==orderStateId);
                }
                if (auditStatusId != null)
                {
                    entities = entities.Where(a => a.AuditStatusId == auditStatusId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g =>g.Code.Contains(keyword) ||  g.Buyer.Mobile.Contains(keyword));
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
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<OrderSearchResult> GetRefundModelListAsync(long? buyerId, int? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                var entities = dbc.GetAll<OrderEntity>().Include(o => o.Address).Where(o => o.OrderStateId.GetEnumName<OrderStateEnum>() == "退单审核" || o.OrderStateId.GetEnumName<OrderStateEnum>() == "退单中" || o.OrderStateId.GetEnumName<OrderStateEnum>() == "退单完成");
                if (buyerId != null)
                {
                    entities = entities.Where(a => a.BuyerId == buyerId);
                }
                if (orderStateId != null)
                {
                    entities = entities.Where(a => a.OrderStateId == orderStateId);
                }
                if (auditStatusId != null)
                {
                    entities = entities.Where(a => a.AuditStatusId == auditStatusId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Buyer.Mobile.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", endTime, a.ApplyTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var goodsTypesResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<OrderSearchResult> GetReturnModelListAsync(long? buyerId, int? orderStateId, long? auditStatusId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                var entities = dbc.GetAll<OrderEntity>().Include(o => o.Address).Where(o => o.OrderStateId.GetEnumName<OrderStateEnum>() == "退货审核" || o.OrderStateId.GetEnumName<OrderStateEnum>() == "退货中" || o.OrderStateId.GetEnumName<OrderStateEnum>() == "退货完成");
                if (buyerId != null)
                {
                    entities = entities.Where(a => a.BuyerId == buyerId);
                }
                if (orderStateId != null)
                {
                    entities = entities.Where(a => a.OrderStateId == orderStateId);
                }
                if (auditStatusId != null)
                {
                    entities = entities.Where(a => a.AuditStatusId == auditStatusId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Buyer.Mobile.Contains(keyword));
                }
                if (startTime != null)
                {
                    entities = entities.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    entities = entities.Where(a => SqlFunctions.DateDiff("day", endTime, a.ApplyTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await entities.LongCountAsync()) * 1.0f / pageSize);
                var goodsTypesResult = await entities.OrderByDescending(a => a.ApplyTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<OrderSearchResult> GetDeliverModelListAsync(long? buyerId, int? orderStateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderSearchResult result = new OrderSearchResult();
                var entities = dbc.GetAll<OrderEntity>().Include(o => o.Address).Where(o => o.OrderStateId == (int)OrderStateEnum.待发货 || o.OrderStateId == (int)OrderStateEnum.已发货);
                if (buyerId != null)
                {
                    entities = entities.Where(a => a.BuyerId == buyerId);
                }
                if (orderStateId !=null)
                {
                    entities = entities.Where(a => a.OrderStateId==orderStateId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Code.Contains(keyword) || g.Buyer.Mobile.Contains(keyword));
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
                result.Orders = goodsTypesResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> UpdateAsync(long id, long? addressId, int? payTypeId, int? orderStateId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                if(addressId!=null)
                {
                    entity.AddressId = addressId.Value;
                }
                if (payTypeId != null)
                {
                    entity.PayTypeId = payTypeId.Value;
                }
                if (orderStateId != null)
                {
                    entity.OrderStateId = orderStateId.Value;
                }
                await dbc.SaveChangesAsync();
                return true;
            }           
        }

        public async Task<long> UpdateDeliverStateAsync(long id,string deliver, string deliverName, string deliverCode)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity entity = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                else
                {
                    entity.OrderStateId = (int)OrderStateEnum.已发货;
                }
                //entity.Deliver = deliver;
                entity.DeliverName = deliverName;
                entity.DeliverCode = deliverCode;
                entity.ConsignTime = DateTime.Now;
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == entity.BuyerId);
                if (user == null)
                {
                    return -3;
                }

                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<bool> Receipt(long id, int orderStateId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o=>o.Id==id);
                if(order==null)
                {
                    return false;
                }
                order.EndTime = DateTime.Now;
                order.OrderStateId = orderStateId;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> ApplyReturnOrderAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                //if (order == null)
                //{
                //    return -1;
                //}
                //if(order.OrderStateId!=(int)OrderStateEnum.待发货)
                //{
                //    return -2;
                //}
                //order.ApplyTime = DateTime.Now;
                //order.AuditStatusId = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "未审核");
                //order.OrderStateId = (int)OrderStateEnum.退单审核;
                //await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> ReturnOrderAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                //if (order == null)
                //{
                //    return -1;
                //}
                //UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                //if(user==null)
                //{
                //    return -2;
                //}

                //var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                //foreach (var orderlist in orderlists)
                //{
                //    GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == orderlist.GoodsId);
                //    if (goods == null)
                //    {
                //        continue;
                //    }
                //    goods.Inventory = goods.Inventory + orderlist.Number;
                //    goods.SaleNum = goods.SaleNum - orderlist.Number;
                //}

                //JournalEntity journal1 = await dbc.GetAll<JournalEntity>().SingleOrDefaultAsync(j => j.UserId == user.Id && j.OrderCode == order.Code && j.JournalType.Name== "购物");
                //if(journal1==null)
                //{
                //    return -3;
                //}
                //if(order.OrderStateId!= (int)OrderStateEnum.退单中)
                //{
                //    return -4;
                //}
                //order.OrderStateId= (int)OrderStateEnum.退单完成;

                //user.Amount = user.Amount + journal1.OutAmount.Value;
                //user.BuyAmount = user.BuyAmount - journal1.OutAmount.Value;

                //var journals = await dbc.GetAll<JournalEntity>().Where(j => j.OrderCode == order.Code && j.JournalType.Name == "佣金收入" && j.IsEnabled == false).ToListAsync();
                //foreach (JournalEntity journal2 in journals)
                //{
                //    UserEntity user1 = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == journal2.UserId);
                //    user1.FrozenAmount = user1.FrozenAmount - journal2.InAmount.Value;
                //    //journal2.IsEnabled = true;
                //    journal2.IsDeleted = true;
                //}

                ////添加流水记录
                //JournalEntity journal = new JournalEntity();
                //journal.OrderCode = order.Code;
                //journal.UserId = user.Id;
                //journal.InAmount = journal1.OutAmount.Value;
                //journal.Remark = "退单退款";
                //journal.JournalTypeId = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "退单退款");
                //journal.BalanceAmount = user.Amount;
                //dbc.Journals.Add(journal);
                //await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> ApplyReturnAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                //if (order == null)
                //{
                //    return -1;
                //}
                //if(order.OrderStateId!= (int)OrderStateEnum.已完成)
                //{
                //    return -2;
                //}
                //if(order.CloseTime!=null)
                //{
                //    return -6;
                //}
                //string val = await dbc.GetStringPropertyAsync<SettingEntity>(s=>s.Name== "不能退货时间", s=>s.Param);

                //if (order.EndTime!=null && DateTime.Now > order.EndTime.Value.AddDays(Convert.ToDouble(val)))
                //{
                //    return -4;
                //}
                //var orderLists = await dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToListAsync();
                //decimal totalAmount = 0;
                //decimal totalReturnAmount = 0;
                //decimal totalDiscountReturnAmount = 0;
                //decimal totalDiscountAmount = 0;
                //if(!orderLists.Any(o=>o.IsReturn==true))
                //{
                //    return -5;
                //}
                //foreach (var item in orderLists)
                //{
                //    totalAmount = totalAmount + item.TotalFee;
                //    if (item.IsReturn == true)
                //    {
                //        totalReturnAmount = totalReturnAmount + item.TotalFee;
                //    }
                //}                
                //if (order.DiscountAmount != 0 && totalAmount != 0 && order.UpAmount!=null)
                //{
                //    totalDiscountAmount = totalAmount * order.UpAmount.Value;
                //    totalDiscountReturnAmount = totalReturnAmount * order.UpAmount.Value;
                //}
                //if (totalDiscountReturnAmount <= 0)
                //{
                //    return -2;
                //}
                //decimal percent = Convert.ToDecimal(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == "退货扣除比例", s => s.Param)) / 100;
                //order.ApplyTime = DateTime.Now;
                //order.ReturnAmount = totalDiscountReturnAmount;
                //order.DeductAmount = totalDiscountReturnAmount * percent;
                //order.RefundAmount = order.ReturnAmount - order.DeductAmount;
                //order.DownCycledId = await dbc.GetEntityIdAsync<IdNameEntity>(i=>i.Name== "--不降级");
                //order.AuditStatusId= await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "未审核"); 
                //order.OrderStateId = (int)OrderStateEnum.退货审核;
                //UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                //if (user == null)
                //{
                //    return -3;
                //}

                ////黄金会员id
                //long levelId1 = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "黄金会员");
                //long levelId2 = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "铂金会员");
                //if (user.LevelId == levelId1 && !user.IsReturned)
                //{
                //    if (totalReturnAmount / totalAmount >= (decimal)0.5)
                //    {
                //        order.DownCycledId = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "↓普通会员");
                //    }
                //}
                //if (user.LevelId == levelId2 && !user.IsReturned)
                //{
                //    if (totalReturnAmount / totalAmount >= (decimal)0.5)
                //    {
                //        order.DownCycledId = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "↓普通会员");
                //    }
                //}
                //await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> ReturnAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o=>o.Id==orderId);
                //if(order==null)
                //{
                //    return -1;
                //}

                //var orderLists = await dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToListAsync();

                //if (!orderLists.Any(o => o.IsReturn == true))
                //{
                //    return -2;
                //}

                //foreach (var item in orderLists)
                //{
                //    if (item.IsReturn == true)
                //    {
                //        var journals = await dbc.GetAll<JournalEntity>().Where(j =>j.GoodsId==item.GoodsId && j.OrderCode == order.Code && j.JournalType.Name == "佣金收入" && j.IsEnabled == false).ToListAsync();
                        
                //        foreach (JournalEntity journal1 in journals)
                //        {
                //            UserEntity user1 = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == journal1.UserId);
                //            user1.FrozenAmount = user1.FrozenAmount - journal1.InAmount.Value;
                //            //journal1.IsEnabled = true;
                //            journal1.IsDeleted = true;
                //        }                        
                //    }
                //    else
                //    {
                //        var journals = await dbc.GetAll<JournalEntity>().Where(j => j.GoodsId == item.GoodsId && j.OrderCode == order.Code && j.JournalType.Name == "佣金收入" && j.IsEnabled == false).ToListAsync();

                //        foreach (JournalEntity journal1 in journals)
                //        {
                //            UserEntity user1 = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == journal1.UserId);
                //            user1.Amount = user1.Amount + journal1.InAmount.Value;
                //            user1.FrozenAmount = user1.FrozenAmount - journal1.InAmount.Value;
                //            user1.BonusAmount = user1.BonusAmount + journal1.InAmount.Value;
                //            journal1.BalanceAmount = user1.Amount;
                //            journal1.IsEnabled = true;
                //        }
                //    }
                //}
                
                //order.OrderStateId = (int)OrderStateEnum.退货完成;
                //UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u=>u.Id==order.BuyerId);
                //if(user==null)
                //{
                //    return -3;
                //}

                ////会员扣除金额、降级
                //user.Amount = user.Amount + order.RefundAmount.Value;
                //user.BuyAmount = user.BuyAmount - order.RefundAmount.Value;
                ////普通会员id
                //int levelId = 1;

                //if (order.DownCycledId == await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "↓普通会员"))
                //{
                //    user.LevelId = levelId;
                //    user.IsReturned = true;
                //}

                ////添加流水记录
                //JournalEntity journal = new JournalEntity();
                //journal.OrderCode = order.Code;
                //journal.UserId = user.Id;
                //journal.InAmount = order.RefundAmount;
                //journal.Remark = "退货退款";
                //journal.JournalTypeId = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "退货退款");
                //journal.BalanceAmount = user.Amount;
                //dbc.Journals.Add(journal);
                //await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<long> RefundAuditAsync(long orderId, long adminId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                //if (order == null)
                //{
                //    return -1;
                //}
                //order.AuditStatusId = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "已审核");
                //order.OrderStateId = (int)OrderStateEnum.退单中;
                //order.AuditMobile = await dbc.GetStringPropertyAsync<AdminEntity>(a => a.Id == adminId,a=>a.Mobile);
                //order.AuditTime = DateTime.Now;
                //await dbc.SaveChangesAsync();
                //return order.Id;
                return 1;
            }
        }

        public async Task<long> ReturnAuditAsync(long orderId, long adminId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
                //if (order == null)
                //{
                //    return -1;
                //}
                //if(!(await dbc.GetAll<OrderListEntity>().AnyAsync(o => o.IsReturn == true)))
                //{
                //    return -2;
                //}
                //order.AuditStatusId = await dbc.GetEntityIdAsync<IdNameEntity>(i => i.Name == "已审核");
                //order.OrderStateId = (int)OrderStateEnum.退货中;
                //order.AuditMobile = await dbc.GetStringPropertyAsync<AdminEntity>(a => a.Id == adminId, a => a.Mobile);
                //order.AuditTime = DateTime.Now;
                //await dbc.SaveChangesAsync();
                //return order.Id;
                return 1;
            }
        }

        public async Task AutoConfirmAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                int stateId = (int)OrderStateEnum.已完成;
                string val = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == "自动确认收货时间", s => s.Param);
                double day;
                double.TryParse(val, out day);
                if (day == 0)
                {
                    day = 7;
                }
                var orders = dbc.GetAll<OrderEntity>().AsNoTracking().Where(r => r.OrderStateId == (int)OrderStateEnum.已发货).Where(r => SqlFunctions.DateAdd("day", day, r.ConsignTime) < DateTime.Now);
                foreach (OrderEntity order in orders)
                {
                    order.EndTime = DateTime.Now;
                    order.OrderStateId = stateId;
                }
                //val = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.Name == "不能退货时间", s => s.Param);
                //double.TryParse(val, out day);
                //if (day == 0)
                //{
                //    day = 3;
                //}
                //var orders1 = dbc.GetAll<OrderEntity>().Where(r => r.CloseTime == null).Where(r => r.OrderStateId == (int)OrderStateEnum.已完成 || r.OrderStateId == (int)OrderStateEnum.退货审核).Where(r => SqlFunctions.DateAdd("day", day, r.EndTime) < DateTime.Now);
                //List<string> orderCodes = new List<string>();
                //foreach (OrderEntity order in orders1)
                //{
                //    order.OrderStateId = stateId;
                //    order.CloseTime = DateTime.Now;
                //    orderCodes.Add(order.Code);
                //}
                //var journals = dbc.GetAll<JournalEntity>().AsNoTracking().Where(j => orderCodes.Contains(j.OrderCode) && j.JournalType.Name == "佣金收入" && j.IsEnabled == false);
                //Dictionary<long, long> dicts = new Dictionary<long, long>();
                //foreach (JournalEntity journal in journals)
                //{
                //    dicts.Add(journal.Id, journal.UserId);
                //}
                //foreach (var dict in dicts)
                //{
                //    var user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == dict.Value);
                //    JournalEntity journal = await dbc.GetAll<JournalEntity>().SingleOrDefaultAsync(j => j.Id == dict.Key);
                //    user.Amount = user.Amount + journal.InAmount.Value;
                //    user.FrozenAmount = user.FrozenAmount - journal.InAmount.Value;
                //    user.BonusAmount = user.BonusAmount + journal.InAmount.Value;
                //    journal.BalanceAmount = user.Amount;
                //    journal.IsEnabled = true;
                //}

                await dbc.SaveChangesAsync();
            }
        }

        public void AutoConfirm()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                int stateId = (int)OrderStateEnum.已完成;
                string val = dbc.GetStringProperty<SettingEntity>(s => s.Name == "自动确认收货时间", s => s.Param);
                double day;
                double.TryParse(val, out day);
                if (day == 0)
                {
                    day = 7;
                }

                //Expression<Func<OrderEntity, bool>> timewhere = r => r.ConsignTime == null ? false : r.ConsignTime.Value.AddDays(Convert.ToDouble(val)) < DateTime.Now;
                //var orders = dbc.GetAll<OrderEntity>().Where(r => r.OrderState.Name == "已发货").Where(timewhere.Compile()).ToList();
                var orders = dbc.GetAll<OrderEntity>().Where(r => r.OrderStateId == (int)OrderStateEnum.已发货).Where(r => SqlFunctions.DateAdd("day", day, r.ConsignTime) < DateTime.Now);
                long count = 0;
                foreach (OrderEntity order in orders)
                {
                    order.EndTime = DateTime.Now;
                    order.OrderStateId = stateId;
                    count++;
                }
                log.Debug($"自动确认收货时间:{day},订单状态:{stateId},符合要求订单数量{count}");
                dbc.SaveChanges();
                //val = dbc.GetStringProperty<SettingEntity>(s => s.Name == "不能退货时间", s => s.Param);
                //double.TryParse(val, out day);
                //if (day == 0)
                //{
                //    day = 3;
                //}
                DateTime time = DateTime.Now.AddMinutes(-30);
                var orders1 = dbc.GetAll<OrderEntity>().Where(r => r.OrderStateId == (int)OrderStateEnum.待付款).Where(r => r.CreateTime < time);
                long count1 = orders1.Count();
                stateId = (int)OrderStateEnum.已取消;
                foreach (OrderEntity order in orders1)
                {
                    order.OrderStateId = stateId;
                }

                dbc.SaveChanges();
                log.Debug($"service中执行自动收货，自动取消订单数量"+count1+",完成");
            }
        }

        public async Task<long> ValidOrder(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    return -1;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                if (user == null)
                {
                    return -2;
                }
                if(!user.IsEnabled)
                {
                    return -6;
                }
                var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                foreach (var orderlist in orderlists)
                {
                    GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == orderlist.GoodsId);
                    if (goods == null)
                    {
                        continue;
                    }
                    if(!goods.IsPutaway)
                    {
                        return -4;
                    }
                    if (goods.Inventory < orderlist.Number)
                    {
                        return -3;
                    }
                }
                return 1;
            }
        }

        public async Task<decimal[]> GetTotalAmountAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string strtime = DateTime.Now.Year.ToString();
                List<decimal> list = new List<decimal>();
                for(int i=1;i<=12;i++)
                {
                    DateTime time = Convert.ToDateTime(strtime + "-" + i);
                    var res = dbc.GetAll<OrderEntity>().Where(o => o.CreateTime.Year == time.Year && o.CreateTime.Month==time.Month);
                    if(await res.CountAsync()<=0)
                    {
                        continue;
                    }
                    var result = await res.SumAsync(o=>o.Amount);
                    list.Add(result);
                }
                return list.ToArray();
            }
        }

        public async Task<CalcDataModel> GetDataAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CalcDataModel model = new CalcDataModel();
                DateTime time = DateTime.Now;
                model.RegisterCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.CreateTime.Year == time.Year && u.CreateTime.Month == time.Month && u.CreateTime.Day == time.Day).CountAsync();
                var res = dbc.GetAll<OrderEntity>().AsNoTracking().Where(o=>o.OrderStateId>(int)OrderStateEnum.待付款 && o.OrderStateId<(int)OrderStateEnum.已取消).Where(u => u.CreateTime.Year == time.Year && u.CreateTime.Month == time.Month && u.CreateTime.Day == time.Day);
                model.OrderCount = await res.CountAsync();
                model.ApplyTakeCashCount= await dbc.GetAll<TakeCashEntity>().AsNoTracking().Where(u=>u.StateId==(int)TakeCashStateEnum.未结款).Where(u =>u.CreateTime.Year == time.Year && u.CreateTime.Month == time.Month && u.CreateTime.Day == time.Day).CountAsync();
                if(model.OrderCount>0)
                {
                    model.TotalOrderAmount = await res.SumAsync(o=>o.Amount);
                }
                model.TotalBonusAmount = await dbc.GetAll<UserEntity>().AsNoTracking().SumAsync(a => a.BonusAmount);
                return model;
            }
        }

        public async Task<bool> OrderCancelAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o=>o.Id==orderId);
                if(order==null)
                {
                    return false;
                }
                if (order.OrderStateId != (int)OrderStateEnum.待付款)
                {
                    return false;
                }
                order.OrderStateId = (int)OrderStateEnum.已取消;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
        
        public async Task<bool> EditReceiverInfoAsync(long id, string receiverName, string receiverMobile, string receiverAddress)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == id);
                if (order == null)
                {
                    return false;
                }
                order.ReceiverName = receiverName;
                order.ReceiverMobile = receiverMobile;
                order.ReceiverAddress = receiverAddress;
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
