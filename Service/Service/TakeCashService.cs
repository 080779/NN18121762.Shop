using IMS.Common.Enums;
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
    public class TakeCashService : ITakeCashService
    {
        public TakeCashDTO ToDTO(TakeCashEntity entity, PayCodeDTO payCode, BankAccountDTO bankAccount)
        {
            TakeCashDTO dto = new TakeCashDTO();
            dto.Amount = entity.Amount;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.Id = entity.Id;
            dto.StateId = entity.StateId;
            dto.StateName = entity.StateId.GetEnumName<TakeCashStateEnum>();
            dto.PayTypeId = entity.PayTypeId;
            dto.PayTypeName = entity.PayTypeId.GetEnumName<ReceiptTypeEnum>();
            dto.PayCode = payCode;
            dto.BankAccount = bankAccount;
            dto.NickName = entity.User.NickName;
            dto.Mobile = entity.User.Mobile;
            dto.UserCode = entity.User.UserCode;
            dto.AdminMobile = entity.AdminMobile;
            dto.Poundage = entity.Poundage;
            return dto;
        }
        public BankAccountDTO ToDTO(BankAccountEntity entity)
        {
            BankAccountDTO dto = new BankAccountDTO();
            if(entity==null)
            {
                return dto = null;
            }
            dto.Name = entity.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.BankAccount = entity.BankAccount;
            dto.BankName = entity.BankName;
            dto.Description = entity.Description;
            dto.UserId = entity.UserId;
            return dto;
        }
        public PayCodeDTO ToDTO(PayCodeEntity entity)
        {
            PayCodeDTO dto = new PayCodeDTO();
            if (entity == null)
            {
                return dto = null;
            }
            dto.Description = entity.Description;
            dto.Name = entity.Name;
            dto.CreateTime = entity.CreateTime;
            dto.Id = entity.Id;
            dto.CodeUrl = entity.CodeUrl;
            dto.UserId = entity.UserId;
            return dto;
        }

        public async Task<long> AddAsync(long userId, int payTypeId, decimal amount,decimal poundage, string descripton)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);
                if(user==null)
                {
                    return -1;
                }
                if(user.Amount<amount)
                {
                    return -2;
                }
                if(string.IsNullOrEmpty(await dbc.GetStringPropertyAsync<BankAccountEntity>(b => b.UserId == userId, b => b.BankAccount)))
                {
                    return -4;
                }
                TakeCashEntity entity = new TakeCashEntity();
                entity.UserId = userId;
                entity.PayTypeId = payTypeId;
                var stateId =(int)TakeCashStateEnum.未结款;
                if(stateId == 0)
                {
                    return -3;
                }
                entity.StateId = stateId;
                entity.Amount = amount;
                entity.Description = descripton;
                entity.Poundage = poundage;
                dbc.TakeCashes.Add(entity);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }
        
        public async Task<long> ConfirmAsync(long id,long adminId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TakeCashEntity takeCash = await dbc.GetAll<TakeCashEntity>().SingleOrDefaultAsync(t=>t.Id==id);
                if(takeCash==null)
                {
                    return -1;
                }
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == takeCash.UserId);
                if(user==null)
                {
                    return -2;
                }
                if(takeCash.Amount>user.Amount)
                {
                    return -3;
                }
                user.Amount = user.Amount - takeCash.Amount;
                takeCash.StateId = (int)TakeCashStateEnum.已结款;
                takeCash.AdminMobile = (await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == adminId)).Mobile;
                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = takeCash.Amount;
                journal.Remark = "用户(" + user.Mobile + ")提现";
                journal.JournalTypeId = (int)JournalTypeEnum.佣金提现;
                //journal.OrderCode = order.Code;
                //journal.GoodsId = order.Id;//来至订单ID
                journal.CurrencyType = 1;//币种,只有rmb
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
                return takeCash.Id;
            }
        }

        public async Task<long> CancelAsync(long id, string description, long adminId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TakeCashEntity takeCash = await dbc.GetAll<TakeCashEntity>().SingleOrDefaultAsync(t => t.Id == id);
                if (takeCash == null)
                {
                    return -1;
                }
                takeCash.StateId = (int)TakeCashStateEnum.已取消;
                takeCash.Description = description;
                takeCash.AdminMobile = (await dbc.GetAll<AdminEntity>().SingleOrDefaultAsync(a => a.Id == adminId)).Mobile;
                await dbc.SaveChangesAsync();
                return takeCash.Id;
            }
        }

        public async Task<TakeCashSearchResult> GetModelListAsync(long? userId,long? stateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                TakeCashSearchResult result = new TakeCashSearchResult();
                var entities = dbc.GetAll<TakeCashEntity>();
                if(userId!=null)
                {
                    entities = entities.Where(a => a.UserId == userId);
                }
                if (stateId != null)
                {
                    entities = entities.Where(a => a.StateId == stateId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(g => g.User.Mobile.Contains(keyword) || g.User.NickName.Contains(keyword));
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
                var takeCashResult = await entities.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.TakeCashes = takeCashResult.Select(a => ToDTO(a,ToDTO(dbc.GetAll<PayCodeEntity>().SingleOrDefault(p => p.UserId == a.UserId)),ToDTO(dbc.GetAll<BankAccountEntity>().SingleOrDefault(b=>b.UserId==a.UserId)))).ToArray();
                return result;
            }
        }
    }
}
