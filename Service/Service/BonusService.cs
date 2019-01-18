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
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Service
{
    public class BonusService : IBonusService
    {
        private static ILog log = LogManager.GetLogger(typeof(OrderService));
        public BonusDTO ToDTO(BonusEntity entity)
        {
            BonusDTO dto = new BonusDTO();
            dto.Source = entity.Source;
            dto.TypeID = entity.TypeID;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.Amount = entity.Amount;
            dto.Revenue = entity.Revenue;
            dto.sf = entity.sf;
            dto.UserId = entity.UserId;
            dto.IsSettled = entity.IsSettled;
            dto.SttleTime = entity.SttleTime;
            dto.FromUserID = entity.FromUserID;
            dto.UserMobile = entity.User.Mobile;
            return dto;
        }
        public async Task<long> AddAsync(long userId, decimal amount, decimal revenue, decimal sf, string source, long fromUserID, int isSettled)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusEntity entity = new BonusEntity();
                entity.UserId = userId;
                entity.Amount = amount;
                entity.Revenue = revenue;
                entity.sf = sf;
                entity.Source = source;
                entity.FromUserID = fromUserID;
                entity.IsSettled = isSettled;
                dbc.Bonus.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusEntity entity = await dbc.GetAll<BonusEntity>().SingleOrDefaultAsync(g => g.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<BonusDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entity = await dbc.GetAll<BonusEntity>().AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<BonusSearchResult> GetModelListAsync(long? userId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusSearchResult result = new BonusSearchResult();
                var entities = dbc.GetAll<BonusEntity>().AsNoTracking();
                if (userId != null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.Source.Contains(keyword));
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
                var addressResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = addressResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }

        }        

        //奖金发放记录
        public async Task<BonusSearchResult> BoussListAsync(long? userId, long? typeId, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusSearchResult result = new BonusSearchResult();
                var logs = dbc.GetAll<BonusEntity>().AsNoTracking();
                if (userId != null && userId > 0)
                {
                    logs = logs.Where(a => a.UserId == userId);
                }
                if (typeId != null && typeId > 0)
                {
                    logs = logs.Where(a => a.TypeID == typeId);
                }
                result.PageCount = (int)Math.Ceiling((await logs.LongCountAsync()) * 1.0f / pageSize);
                var logsResult = await logs.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.List = logsResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }

        public async Task<bool> CalcShareBonusAsync(decimal totalScore)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                decimal term;
                int levelId;
                int count;
                decimal bonusAmount;
                //decimal totalScore = await dbc.GetAll<UserEntity>().AsNoTracking().SumAsync(u => u.Amount);
                IQueryable<UserEntity> users;
                //皇冠会员
                users = dbc.GetAll<UserEntity>().Where(u => u.LevelId == (int)LevelEnum.皇冠会员);
                count = await users.CountAsync();
                if (count > 0)
                {
                    levelId = 1;
                    decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "全国分红" && s.LevelId == levelId, s => s.Param), out term);
                    bonusAmount = totalScore * (term / 100) / count;
                    foreach (UserEntity user in await users.ToListAsync())
                    {
                        user.Amount = user.Amount + bonusAmount;

                        BonusEntity bonus = new BonusEntity();
                        bonus.UserId = user.Id;
                        bonus.Amount = bonusAmount;
                        bonus.Revenue = 0;
                        bonus.sf = bonusAmount;
                        bonus.TypeID = 5; //5、分红
                        bonus.Source = "用户(" + user.Mobile + ")获得全国分红";
                        bonus.FromUserID = user.Id;
                        bonus.IsSettled = 1;
                        dbc.Bonus.Add(bonus);

                        JournalEntity journal = new JournalEntity();
                        journal.UserId = user.Id;
                        journal.BalanceAmount = user.Amount;
                        journal.InAmount = bonusAmount;
                        journal.Remark = "用户(" + user.Mobile + ")获得全国分红";
                        journal.JournalTypeId = (int)JournalTypeEnum.全国分红;
                        //journal.OrderCode = order.Code;
                        //journal.GoodsId = order.Id;//来至订单ID
                        journal.CurrencyType = 1;//币种,只有rmb
                        dbc.Journals.Add(journal);
                    }
                }
                users = dbc.GetAll<UserEntity>().Where(u => u.LevelId == (int)LevelEnum.董事会员);
                count = await users.CountAsync();
                if (count > 0)
                {
                    levelId = 2;
                    decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "全国分红" && s.LevelId == levelId, s => s.Param), out term);
                    bonusAmount = totalScore * (term / 100) / count;
                    foreach (UserEntity user in await users.ToListAsync())
                    {
                        user.Amount = user.Amount + bonusAmount;
                        user.BonusAmount = user.BonusAmount + bonusAmount;

                        BonusEntity bonus = new BonusEntity();
                        bonus.UserId = user.Id;
                        bonus.Amount = bonusAmount;
                        bonus.Revenue = 0;
                        bonus.sf = bonusAmount;
                        bonus.TypeID = 5; //5、分红
                        bonus.Source = "用户(" + user.Mobile + ")获得全国分红";
                        bonus.FromUserID = user.Id;
                        bonus.IsSettled = 1;
                        dbc.Bonus.Add(bonus);

                        JournalEntity journal = new JournalEntity();
                        journal.UserId = user.Id;
                        journal.BalanceAmount = user.Amount;
                        journal.InAmount = bonusAmount;
                        journal.Remark = "用户(" + user.Mobile + ")获得全国分红";
                        journal.JournalTypeId = (int)JournalTypeEnum.全国分红;
                        //journal.OrderCode = order.Code;
                        //journal.GoodsId = order.Id;//来至订单ID
                        journal.CurrencyType = 1;//币种,只有rmb
                        dbc.Journals.Add(journal);
                    }
                }
                await dbc.SaveChangesAsync();
                return true;                
            }
        }
    }
}
