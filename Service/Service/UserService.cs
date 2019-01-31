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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static IMS.Common.Enums.MyEnumHelper;

namespace IMS.Service.Service
{
    public class UserService : IUserService
    {
        private static ILog log = LogManager.GetLogger(typeof(UserService));
        public UserDTO ToDTO(UserEntity entity)
        {
            UserDTO dto = new UserDTO();
            dto.Amount = entity.Amount;
            dto.UserCode = entity.UserCode;
            dto.CreateTime = entity.CreateTime;
            dto.Description = entity.Description;
            dto.ErrorCount = entity.ErrorCount;
            dto.ErrorTime = entity.ErrorTime;
            dto.Id = entity.Id;
            dto.IsEnabled = entity.IsEnabled;
            dto.LevelId = entity.LevelId;
            dto.LevelName = entity.LevelId.GetEnumName<LevelEnum>();
            dto.Mobile = entity.Mobile;
            dto.NickName = entity.NickName;
            dto.BuyAmount = entity.BuyAmount;
            dto.IsReturned = entity.IsReturned;
            dto.IsUpgraded = entity.IsUpgraded;
            dto.BonusAmount = entity.BonusAmount;
            dto.HeadPic = entity.HeadPic;
            dto.ShareCode = entity.ShareCode;
            dto.FrozenAmount = entity.FrozenAmount;
            dto.RecommendCode = entity.RecommendCode;
            dto.RecommendGenera = entity.RecommendGenera;
            dto.RecommendId = entity.RecommendId;
            dto.RecommendPath = entity.RecommendPath;
            return dto;
        }

        public async Task<long> AddAsync(string mobile, string password, string tradePassword, int levelTypeId, string recommendCode, string nickName, string avatarUrl)
        {
            string userCode = string.Empty;

            using (MyDbContext dbc = new MyDbContext())
            {
                long userId = 0;
                do
                {
                    userCode = CommonHelper.GetNumberCaptcha(6);
                    userId = await dbc.GetEntityIdAsync<UserEntity>(u => u.UserCode == userCode);
                } while (userId != 0);

                UserEntity recUser;
                if (string.IsNullOrWhiteSpace(recommendCode))
                {
                    recUser = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Id == 1);
                }
                else
                {
                    recUser = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Mobile == recommendCode);
                }

                if (recUser == null)
                {
                    return -1;
                }

                //if(!recUser.IsUpgraded)
                //{
                //    return -4;
                //}

                if ((await dbc.GetEntityIdAsync<UserEntity>(u => u.Mobile == mobile)) > 0)
                {
                    return -2;
                }

                try
                {
                    UserEntity user = new UserEntity();
                    user.UserCode = userCode;
                    user.LevelId = levelTypeId;
                    user.Mobile = mobile;
                    user.Salt = CommonHelper.GetCaptcha(4);
                    user.Password = CommonHelper.GetMD5(password + user.Salt);
                    user.TradePassword = CommonHelper.GetMD5(tradePassword + user.Salt);;// tradePassword;// CommonHelper.GetMD5(tradePassword + user.Salt);
                    user.NickName = string.IsNullOrEmpty(nickName) ? "无昵称" : nickName;
                    user.HeadPic = string.IsNullOrEmpty(avatarUrl) ? "/images/headpic.png" : avatarUrl;

                    user.RecommendId = recUser.Id;
                    user.RecommendGenera = recUser.RecommendGenera + 1;
                    user.RecommendPath = recUser.RecommendPath;
                    user.RecommendCode = recUser.Mobile;

                    dbc.Users.Add(user);
                    await dbc.SaveChangesAsync();

                    user.RecommendPath = user.RecommendPath + "-" + user.Id;
                    await dbc.SaveChangesAsync();
                    return user.Id;
                }
                catch (Exception ex)
                {
                    //  scope.Rollback();
                    log.ErrorFormat("AddAsync:{0}", ex.ToString());
                    return -3;
                }
            }
        }

        public async Task<bool> AddAmountAsync(string mobile, decimal amount)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (user == null)
                {
                    return false;
                }
                user.Amount = user.Amount + amount;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<int> ChargeAsync(long id, int type, decimal amount)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return -1;
                }
                if(type==1)
                {
                    user.Amount = user.Amount + amount;
                    user.BonusAmount = user.BonusAmount + amount;

                    JournalEntity journal = new JournalEntity();
                    journal.UserId = user.Id;
                    journal.BalanceAmount = user.Amount;
                    journal.InAmount = amount;
                    journal.Remark = "后台增加用户(" + user.Mobile + ")余额";
                    journal.JournalTypeId = (int)JournalTypeEnum.后台充值;
                    journal.OrderCode = "";
                    journal.GoodsId = 0;//来至订单ID
                    journal.CurrencyType = 1;//币种,只有rmb
                    dbc.Journals.Add(journal);
                }
                else
                {
                    if (user.Amount < amount)
                    {
                        return -2;
                    }
                    user.Amount = user.Amount - amount;
                    user.BonusAmount = user.BonusAmount - amount;

                    JournalEntity journal = new JournalEntity();
                    journal.UserId = user.Id;
                    journal.BalanceAmount = user.Amount;
                    journal.InAmount = amount;
                    journal.Remark = "后台扣除用户(" + user.Mobile + ")余额";
                    journal.JournalTypeId = (int)JournalTypeEnum.后台充值;
                    journal.OrderCode = "";
                    journal.GoodsId = 0;//来至订单ID
                    journal.CurrencyType = 1;//币种,只有rmb
                    dbc.Journals.Add(journal);
                }
                await dbc.SaveChangesAsync();
                return 1;
            }
        }

        public async Task<bool> UpdateInfoAsync(long id, string nickName, string headpic)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                if (nickName != null)
                {
                    entity.NickName = nickName;
                }
                if (headpic != null)
                {
                    entity.HeadPic = headpic;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UpdateShareCodeAsync(long id, string codeUrl)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.ShareCode = codeUrl;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> AddRecommendAsync(long userId, string recommendMobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //UserEntity user = await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Id == userId);
                //long recommendId = (await dbc.GetAll<UserEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.Mobile == recommendMobile)).Id;
                //RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().Where(u => u.IsNull == false).SingleOrDefaultAsync(u => u.UserId == recommendId);
                //if (user == null)
                //{
                //    return -1;
                //}
                //if (recommend == null)
                //{
                //    return -2;
                //}
                //RecommendEntity ruser = new RecommendEntity();
                //ruser.UserId = userId;
                //ruser.RecommendId = recommendId;
                //ruser.RecommendGenera = recommend.RecommendGenera + 1;
                //ruser.RecommendPath = recommend.RecommendPath + "-" + userId;

                //dbc.Recommends.Add(ruser);
                //await dbc.SaveChangesAsync();
                //return user.Id;
                return 0;
            }
        }
        public async Task<bool> DeleteAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                var address = dbc.GetAll<AddressEntity>().Where(a => a.UserId == id);
                if (address.LongCount() > 0)
                {
                    await address.ForEachAsync(a => a.IsDeleted = true);
                }
                var bankAccounts = dbc.GetAll<BankAccountEntity>().Where(a => a.UserId == id);
                if (bankAccounts.LongCount() > 0)
                {
                    await bankAccounts.ForEachAsync(a => a.IsDeleted = true);
                }
                entity.IsDeleted = true;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> FrozenAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return false;
                }
                entity.IsEnabled = !entity.IsEnabled;
                await dbc.SaveChangesAsync();
                return true;
            }
        }

        public async Task<long> ResetPasswordAsync(long id, string password, string newPassword)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.Password != CommonHelper.GetMD5(password + entity.Salt))
                {
                    return -2;
                }
                entity.Password = CommonHelper.GetMD5(newPassword + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetPasswordAsync(long id, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return -1;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetPasswordAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                entity.Password = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task<long> ResetTradePasswordAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                entity.TradePassword = CommonHelper.GetMD5(password + entity.Salt);
                await dbc.SaveChangesAsync();
                return entity.Id;
            }
        }


        public async Task<long> UserCheck(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long id = await dbc.GetEntityIdAsync<UserEntity>(u => u.Mobile == mobile);
                if (id == 0)
                {
                    return -1;
                }
                return id;
            }
        }

        public async Task<long> CheckLoginAsync(string mobile, string password)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (entity == null)
                {
                    return -1;
                }
                if (entity.Password != CommonHelper.GetMD5(password + entity.Salt))
                {
                    return -2;
                }
                if (entity.IsEnabled == false)
                {
                    return -3;
                }
                return entity.Id;
            }
        }

        public async Task<long> CheckTradePasswordAsync(long id, string tradePassword)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return -1;
                }
                if (user.TradePassword != CommonHelper.GetMD5(tradePassword + user.Salt))
                {
                    return -2;
                }
                return 1;
            }
        }

        public bool CheckUserId(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                long res = dbc.GetEntityId<UserEntity>(u => u.Id == id);
                if (res == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<long> BalancePayAsync(long orderId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Id == orderId);
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
                decimal totalAmount = 0;
                foreach (var orderlist in orderlists)
                {
                    GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == orderlist.GoodsId);
                    totalAmount = totalAmount + orderlist.TotalFee;

                    if (goods == null)
                    {
                        continue;
                    }

                    if (!goods.IsPutaway)
                    {
                        return -5;
                    }

                    if (goods.Inventory < orderlist.Number)
                    {
                        return -3;
                    }

                    //商品销量、库存
                    goods.Inventory = goods.Inventory - orderlist.Number;
                    goods.SaleNum = goods.SaleNum + orderlist.Number;
                }
                user.Amount = user.Amount - order.Amount;
                user.BuyAmount = user.BuyAmount + order.Amount;
                user.OrderCount = user.OrderCount + 1;

                order.PayTime = DateTime.Now;
                order.PayTypeId = (int)PayTypeEnum.微信;//余额
                order.OrderStateId = (int)OrderStateEnum.待发货;

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.InAmount = order.Amount;
                journal.Remark = "用户(" + user.Mobile + ")购买商品";
                journal.JournalTypeId = (int)JournalTypeEnum.购买商品;
                journal.OrderCode = order.Code;
                journal.GoodsId = order.Id;//来至订单ID
                journal.CurrencyType = 1;//币种,只有rmb
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();

                if (user.LevelId == (int)LevelEnum.用户)
                {
                    //购买用户升级
                    await UserLevelUpAsync(dbc, user, order);
                }
                else
                {
                    if (user.BuyType == (int)BuyTypeEnum.首次购买)
                    {
                        user.BuyType = (int)BuyTypeEnum.再次购买;
                        await dbc.SaveChangesAsync();
                    }
                }

                long recommendId = user.RecommendId;
                while (recommendId > 0)
                {
                    UserEntity recUser = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == recommendId);

                    switch (recUser.LevelId)
                    {
                        case (int)LevelEnum.会员: await MemberLevelUpAsync(dbc, recUser); break;
                        case (int)LevelEnum.银卡会员: await SilverMemberLevelUpAsync(dbc, recUser); break;
                        case (int)LevelEnum.黄金会员: await GoldMemberLevelUpAsync(dbc, recUser); break;
                        case (int)LevelEnum.钻石会员: await DiamondMemberLevelUpAsync(dbc, recUser); break;
                        case (int)LevelEnum.至尊会员: await ExtremeMemberLevelUpAsync(dbc, recUser); break;
                        case (int)LevelEnum.皇冠会员: await CrownMemberLevelUpAsync(dbc, recUser); break;
                    }
                    recommendId = recUser.RecommendId;
                }

                //发放奖励
                await CalcAwardRecommendAsync(dbc, user, order);
                return 1;
            }
        }

        #region 用户升级方法
        //购买用户升级
        private async Task UserLevelUpAsync(MyDbContext dbc, UserEntity user, OrderEntity order)
        {
            //直推会员数量
            int recCount;
            //直推会员购买订单数量
            int recOrderCount;
            //升级条件1
            decimal term1;
            //升级条件2
            decimal term2;
            decimal term3;
            //升级会员
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "会员升级条件" && s.LevelId == 1, s => s.Param), out term1);
            if (order.Amount < term1)
            {
                return;
            }
            user.LevelId = (int)LevelEnum.会员;
            user.BuyType = (int)BuyTypeEnum.首次购买;

            var res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id);
            if (await res.CountAsync() <= 0)
            {
                recCount = 0;
                recOrderCount = 0;
                //升级银卡会员   
                decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "银卡会员升级条件" && s.LevelId == 1, s => s.Param), out term2);
                if (order.Amount >= term2)
                {
                    user.LevelId = (int)LevelEnum.银卡会员;
                    user.BuyType = (int)BuyTypeEnum.首次购买;
                }
            }
            else
            {
                recCount = await res.CountAsync(u => u.LevelId == (int)LevelEnum.会员);
                recOrderCount = await res.SumAsync(u => u.OrderCount);
            }

            //升级银卡会员            
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "银卡会员升级条件" && s.LevelId == 2, s => s.Param), out term3);
            
            if (recCount >= term3)
            {
                user.LevelId = (int)LevelEnum.银卡会员;
                user.BuyType = (int)BuyTypeEnum.首次购买;
            }

            //升级黄金会员
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "黄金会员升级条件" && s.LevelId == 1, s => s.Param), out term2);
            if (recOrderCount >= term2)
            {
                user.LevelId = (int)LevelEnum.黄金会员;
                user.BuyType = (int)BuyTypeEnum.首次购买;
            }
            await dbc.SaveChangesAsync();
        }
        //会员升级
        private async Task MemberLevelUpAsync(MyDbContext dbc, UserEntity user)
        {
            //直推会员数量
            int recCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id && u.LevelId == (int)LevelEnum.会员).CountAsync();
            //直推会员购买订单数量
            int recOrderCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id).SumAsync(u => u.OrderCount);
            //升级条件
            decimal term;

            //升级银卡会员
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "银卡会员升级条件" && s.LevelId == 2, s => s.Param), out term);
            if (recCount >= term && user.LevelId< (int)LevelEnum.银卡会员)
            {
                user.LevelId = (int)LevelEnum.银卡会员;
            }

            //升级黄金会员
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "黄金会员升级条件" && s.LevelId == 1, s => s.Param), out term);
            if (recOrderCount >= term && user.LevelId < (int)LevelEnum.黄金会员)
            {
                user.LevelId = (int)LevelEnum.黄金会员;
            }
            await dbc.SaveChangesAsync();
        }
        //银卡会员升级
        private async Task SilverMemberLevelUpAsync(MyDbContext dbc, UserEntity user)
        {
            //直推会员购买订单数量
            int recOrderCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id).SumAsync(u => u.OrderCount);
            //升级条件2
            decimal term;

            //升级黄金会员
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "黄金会员升级条件" && s.LevelId == 1, s => s.Param), out term);
            if (recOrderCount >= term && user.LevelId < (int)LevelEnum.黄金会员)
            {
                user.LevelId = (int)LevelEnum.黄金会员;
            }
            await dbc.SaveChangesAsync();
        }
        //黄金会员升级
        private async Task GoldMemberLevelUpAsync(MyDbContext dbc, UserEntity user)
        {
            //直推黄金会员数量
            int recCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id && u.LevelId == (int)LevelEnum.黄金会员).CountAsync();
            //市场业绩
            decimal teamScore;
            //最大业绩
            decimal maxScore;
            IQueryable<UserEntity> res;
            string keyword;
            //升级条件1
            decimal term1;
            //升级条件2
            decimal term2;
            if (user.UserCode == "system")
            {
                keyword = (user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }
            else
            {
                keyword = ("-" + user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }

            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "钻石会员升级条件" && s.LevelId == 1, s => s.Param), out term1);
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "钻石会员升级条件" && s.LevelId == 2, s => s.Param), out term2);

            if (recCount < term1)
            {
                return;
            }
            if (teamScore < term2 * 10000)
            {
                return;
            }
            if(user.LevelId < (int)LevelEnum.钻石会员)
            {
                user.LevelId = (int)LevelEnum.钻石会员;
            }
            await dbc.SaveChangesAsync();
        }
        //钻石会员升级
        private async Task DiamondMemberLevelUpAsync(MyDbContext dbc, UserEntity user)
        {
            //直推钻石会员数量
            int recCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id && u.LevelId == (int)LevelEnum.钻石会员).CountAsync();
            //市场业绩
            decimal teamScore;
            //最大业绩
            decimal maxScore;
            IQueryable<UserEntity> res;
            string keyword;
            //升级条件1
            decimal term1;
            //升级条件2
            decimal term2;
            if (user.UserCode == "system")
            {
                keyword = (user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }
            else
            {
                keyword = ("-" + user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }

            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "至尊会员升级条件" && s.LevelId == 1, s => s.Param), out term1);
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "至尊会员升级条件" && s.LevelId == 2, s => s.Param), out term2);

            if (recCount < term1)
            {
                return;
            }
            if (teamScore < term2 * 10000)
            {
                return;
            }
            if (user.LevelId < (int)LevelEnum.至尊会员)
            {
                user.LevelId = (int)LevelEnum.至尊会员;
            }
            await dbc.SaveChangesAsync();
        }
        //至尊会员升级
        private async Task ExtremeMemberLevelUpAsync(MyDbContext dbc, UserEntity user)
        {
            //直推至尊会员数量
            int recCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id && u.LevelId == (int)LevelEnum.至尊会员).CountAsync();
            //市场业绩
            decimal teamScore;
            //最大业绩
            decimal maxScore;
            IQueryable<UserEntity> res;
            string keyword;
            //升级条件1
            decimal term1;
            //升级条件2
            decimal term2;
            if (user.UserCode == "system")
            {
                keyword = (user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }
            else
            {
                keyword = ("-" + user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }

            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "皇冠会员升级条件" && s.LevelId == 1, s => s.Param), out term1);
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "皇冠会员升级条件" && s.LevelId == 2, s => s.Param), out term2);

            if (recCount < term1)
            {
                return;
            }
            if (teamScore < term2 * 10000)
            {
                return;
            }
            if (user.LevelId < (int)LevelEnum.皇冠会员)
            {
                user.LevelId = (int)LevelEnum.皇冠会员;
            }
            await dbc.SaveChangesAsync();
        }
        //皇冠会员升级
        private async Task CrownMemberLevelUpAsync(MyDbContext dbc, UserEntity user)
        {
            //直推皇冠会员数量
            int recCount = await dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendId == user.Id && u.LevelId == (int)LevelEnum.皇冠会员).CountAsync();
            //市场业绩
            decimal teamScore;
            //最大业绩
            decimal maxScore;
            IQueryable<UserEntity> res;
            string keyword;
            //升级条件1
            decimal term1;
            //升级条件2
            decimal term2;
            if (user.UserCode == "system")
            {
                keyword = (user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }
            else
            {
                keyword = ("-" + user.Id + "-").ToString();
                res = dbc.GetAll<UserEntity>().AsNoTracking().Where(u => u.RecommendPath.Contains(keyword));
                teamScore = await res.SumAsync(u => u.BuyAmount);
                maxScore = await res.MaxAsync(u => u.BuyAmount);
                teamScore = teamScore - maxScore;
            }

            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "董事会员升级条件" && s.LevelId == 1, s => s.Param), out term1);
            decimal.TryParse(await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "董事会员升级条件" && s.LevelId == 2, s => s.Param), out term2);

            if (recCount < term1)
            {
                return;
            }
            if (teamScore < term2)
            {
                return;
            }
            if (user.LevelId < (int)LevelEnum.董事会员)
            {
                user.LevelId = (int)LevelEnum.董事会员;
            }
            await dbc.SaveChangesAsync();
        }
        #endregion

        #region 推荐奖
        private async Task CalcAwardRecommendAsync(MyDbContext dbc, UserEntity user, OrderEntity order)
        {
            if (user.RecommendId == 0)
            {
                return;
            }
            string cashBackType = "";
            switch (user.BuyType)
            {
                case (int)BuyTypeEnum.首次购买: cashBackType = "首购"; break;
                case (int)BuyTypeEnum.再次购买: cashBackType = "复购"; break;
                default: return;
            }
            //直推人
            UserEntity recUser1 = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == user.RecommendId);
            int paramLevelId = 1;
            decimal param;
            string res;
            decimal bonusAmount;
            if (recUser1.LevelId > (int)LevelEnum.用户)
            {
                res = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == cashBackType + "推荐返现" && s.LevelId == paramLevelId, s => s.Param);
                decimal.TryParse(res, out param);
                bonusAmount = order.Amount * param / 100;
                recUser1.Amount = recUser1.Amount + bonusAmount;
                recUser1.BonusAmount = recUser1.BonusAmount + bonusAmount;

                BonusEntity bonus = new BonusEntity();
                bonus.UserId = recUser1.Id;
                bonus.Amount = bonusAmount;
                bonus.Revenue = 0;
                bonus.sf = bonusAmount;
                bonus.TypeID = user.BuyType == ((int)BuyTypeEnum.首次购买) ? 1 : 2; //1、首购直推返现。2、复购直推返现
                bonus.Source = "用户(" + user.Mobile + ")购买商品,用户(" + recUser1.Mobile + ")获得" + cashBackType + "直推返现";
                bonus.FromUserID = user.Id;
                bonus.IsSettled = 1;
                dbc.Bonus.Add(bonus);

                JournalEntity journal = new JournalEntity();
                journal.UserId = recUser1.Id;
                journal.BalanceAmount = recUser1.Amount;
                journal.InAmount = bonusAmount;
                journal.Remark = "用户(" + user.Mobile + ")购买商品,用户(" + recUser1.Mobile + ")获得" + cashBackType + "直推返现";
                journal.JournalTypeId = (int)JournalTypeEnum.推荐奖;
                journal.OrderCode = order.Code;
                journal.GoodsId = order.Id;//来至订单ID
                journal.CurrencyType = 1;//币种,只有rmb
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
            }

            if (recUser1.RecommendId == 0)
            {
                return;
            }

            //间推人
            UserEntity recUser2 = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == recUser1.RecommendId);
            paramLevelId = 2;
            if (recUser2.LevelId > (int)LevelEnum.用户)
            {
                res = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == cashBackType + "推荐返现" && s.LevelId == paramLevelId, s => s.Param);
                decimal.TryParse(res, out param);
                bonusAmount = order.Amount * param / 100;
                recUser2.Amount = recUser2.Amount + bonusAmount;
                recUser2.BonusAmount = recUser2.BonusAmount + bonusAmount;

                BonusEntity bonus1 = new BonusEntity();
                bonus1.UserId = recUser2.Id;
                bonus1.Amount = bonusAmount;
                bonus1.Revenue = 0;
                bonus1.sf = bonusAmount;
                bonus1.TypeID = user.BuyType == ((int)BuyTypeEnum.首次购买) ? 3 : 4; //3、首购间推返现。4、复购间推返现
                bonus1.Source = "用户(" + user.Mobile + ")购买商品,用户(" + recUser2.Mobile + ")获得" + cashBackType + "间推返现";
                bonus1.FromUserID = user.Id;
                bonus1.IsSettled = 1;
                dbc.Bonus.Add(bonus1);

                JournalEntity journal1 = new JournalEntity();
                journal1.UserId = recUser2.Id;
                journal1.BalanceAmount = recUser2.Amount;
                journal1.InAmount = bonusAmount;
                journal1.Remark = "用户(" + user.Mobile + ")购买商品,用户(" + recUser2.Mobile + ")获得" + cashBackType + "间推返现";
                journal1.JournalTypeId = (int)JournalTypeEnum.推荐奖;
                journal1.OrderCode = order.Code;
                journal1.GoodsId = order.Id;//来至订单ID
                journal1.CurrencyType = 1;//币种,只有rmb
                dbc.Journals.Add(journal1);
                await dbc.SaveChangesAsync();
            }
            await CalcAwardUpAsync(dbc, user, order, recUser2.RecommendId);
        }
        #endregion

        #region 晋级奖
        private async Task CalcAwardUpAsync(MyDbContext dbc, UserEntity user, OrderEntity order, long awardRecommendId)
        {
            long recommendId = awardRecommendId;
            decimal bonusAmount;
            decimal param;
            decimal term;
            string res;
            string res1;
            string settingTypeName;
            if (recommendId <= 0)
            {
                return;
            }
            
            //发放晋级奖励
            UserEntity recUser = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == recommendId);
            settingTypeName = GetUpSettingTypeName(recUser.LevelId);
            if (string.IsNullOrEmpty(settingTypeName))
            {
                return;
            }
            if (user.BuyType == (int)BuyTypeEnum.首次购买)
            {
                res = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == settingTypeName && s.LevelId == (int)BuyTypeEnum.首次购买, s => s.Param);
                decimal.TryParse(res, out param);
                bonusAmount = param;
                recUser.Amount = recUser.Amount + bonusAmount;
                recUser.BonusAmount = recUser.BonusAmount + bonusAmount;

                BonusEntity bonus = new BonusEntity();
                bonus.UserId = recUser.Id;
                bonus.Amount = bonusAmount;
                bonus.Revenue = 0;
                bonus.sf = bonusAmount;
                bonus.TypeID = 5; //5、首购晋级
                bonus.Source = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得首购晋级返现";
                bonus.FromUserID = user.Id;
                bonus.IsSettled = 1;
                dbc.Bonus.Add(bonus);

                JournalEntity journal = new JournalEntity();
                journal.UserId = recUser.Id;
                journal.BalanceAmount = recUser.Amount;
                journal.InAmount = bonusAmount;
                journal.Remark = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得首购晋级返现";
                journal.JournalTypeId = (int)JournalTypeEnum.晋级奖;
                journal.OrderCode = order.Code;
                journal.GoodsId = order.Id;//来至订单ID
                journal.CurrencyType = 1;//币种,只有rmb
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
            }
            else if (user.BuyType == (int)BuyTypeEnum.再次购买)
            {
                res = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == settingTypeName && s.LevelId == (int)BuyTypeEnum.再次购买, s => s.Param);
                decimal.TryParse(res, out param);
                bonusAmount = param;
                recUser.Amount = recUser.Amount + bonusAmount;
                recUser.BonusAmount = recUser.BonusAmount + bonusAmount;

                BonusEntity bonus = new BonusEntity();
                bonus.UserId = recUser.Id;
                bonus.Amount = bonusAmount;
                bonus.Revenue = 0;
                bonus.sf = bonusAmount;
                bonus.TypeID = 6; //6、复购晋级
                bonus.Source = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得复购晋级返现";
                bonus.FromUserID = user.Id;
                bonus.IsSettled = 1;
                dbc.Bonus.Add(bonus);

                JournalEntity journal = new JournalEntity();
                journal.UserId = recUser.Id;
                journal.BalanceAmount = recUser.Amount;
                journal.InAmount = bonusAmount;
                journal.Remark = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得复购晋级返现";
                journal.JournalTypeId = (int)JournalTypeEnum.晋级奖;
                journal.OrderCode = order.Code;
                journal.GoodsId = order.Id;//来至订单ID
                journal.CurrencyType = 1;//币种,只有rmb
                dbc.Journals.Add(journal);
                await dbc.SaveChangesAsync();
            }
            recommendId = recUser.RecommendId;

            while (recommendId > 0)
            {
                recUser = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == recommendId);
                settingTypeName = GetUpSettingTypeName(recUser.LevelId);
                if (string.IsNullOrEmpty(settingTypeName))
                {
                    return;
                }
                if (user.BuyType == (int)BuyTypeEnum.首次购买)
                {
                    res = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == settingTypeName && s.LevelId == (int)BuyTypeEnum.首次购买, s => s.Param);
                    res1 = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "管理奖返现比例" && s.LevelId == 1, s => s.Param);
                    decimal.TryParse(res, out param);
                    decimal.TryParse(res1, out term);
                    bonusAmount = param * term / 100;
                    recUser.Amount = recUser.Amount + bonusAmount;
                    recUser.BonusAmount = recUser.BonusAmount + bonusAmount;

                    BonusEntity bonus = new BonusEntity();
                    bonus.UserId = recUser.Id;
                    bonus.Amount = bonusAmount;
                    bonus.Revenue = 0;
                    bonus.sf = bonusAmount;
                    bonus.TypeID = 7; //7、首购管理
                    bonus.Source = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得首购管理返现";
                    bonus.FromUserID = user.Id;
                    bonus.IsSettled = 1;
                    dbc.Bonus.Add(bonus);

                    JournalEntity journal = new JournalEntity();
                    journal.UserId = recUser.Id;
                    journal.BalanceAmount = recUser.Amount;
                    journal.InAmount = bonusAmount;
                    journal.Remark = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得首购管理返现";
                    journal.JournalTypeId = (int)JournalTypeEnum.管理奖;
                    journal.OrderCode = order.Code;
                    journal.GoodsId = order.Id;//来至订单ID
                    journal.CurrencyType = 1;//币种,只有rmb
                    dbc.Journals.Add(journal);
                    await dbc.SaveChangesAsync();
                }
                else if (user.BuyType == (int)BuyTypeEnum.再次购买)
                {
                    res = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == settingTypeName && s.LevelId == (int)BuyTypeEnum.再次购买, s => s.Param);
                    res1 = await dbc.GetStringPropertyAsync<SettingEntity>(s => s.TypeName == "管理奖返现比例" && s.LevelId == 1, s => s.Param);
                    decimal.TryParse(res, out param);
                    decimal.TryParse(res1, out term);
                    bonusAmount = param * term / 100;
                    recUser.Amount = recUser.Amount + bonusAmount;
                    recUser.BonusAmount = recUser.BonusAmount + bonusAmount;

                    BonusEntity bonus = new BonusEntity();
                    bonus.UserId = recUser.Id;
                    bonus.Amount = bonusAmount;
                    bonus.Revenue = 0;
                    bonus.sf = bonusAmount;
                    bonus.TypeID = 8; //8、复购管理
                    bonus.Source = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得复购管理返现";
                    bonus.FromUserID = user.Id;
                    bonus.IsSettled = 1;
                    dbc.Bonus.Add(bonus);

                    JournalEntity journal = new JournalEntity();
                    journal.UserId = recUser.Id;
                    journal.BalanceAmount = recUser.Amount;
                    journal.InAmount = bonusAmount;
                    journal.Remark = "用户(" + user.Mobile + ")购买商品,用户(" + recUser.Mobile + ")获得复购管理返现";
                    journal.JournalTypeId = (int)JournalTypeEnum.管理奖;
                    journal.OrderCode = order.Code;
                    journal.GoodsId = order.Id;//来至订单ID
                    journal.CurrencyType = 1;//币种,只有rmb
                    dbc.Journals.Add(journal);
                    await dbc.SaveChangesAsync();
                }
                recommendId = recUser.RecommendId;
            }
        }
        private string GetUpSettingTypeName(int levelId)
        {
            string str;
            switch (levelId)
            {
                case (int)LevelEnum.银卡会员: str = "银卡会员晋级返现"; break;
                case (int)LevelEnum.黄金会员: str = "黄金会员晋级返现"; break;
                case (int)LevelEnum.钻石会员: str = "钻石会员晋级返现"; break;
                case (int)LevelEnum.至尊会员: str = "至尊会员晋级返现"; break;
                case (int)LevelEnum.皇冠会员: str = "皇冠会员晋级返现"; break;
                case (int)LevelEnum.董事会员: str = "董事会员晋级返现"; break;
                default: str = null; break;
            }
            return str;
        }
        #endregion

        public long WeChatPay(string code)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                OrderEntity order = dbc.GetAll<OrderEntity>().SingleOrDefault(o => o.Code == code);
                if (order == null)
                {
                    return -1;
                }

                if (order.OrderStateId != (int)OrderStateEnum.待付款)
                {
                    return -4;
                }

                UserEntity user = dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == order.BuyerId);
                if (user == null)
                {
                    return -2;
                }

                var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                decimal totalAmount = 0;

                foreach (var orderlist in orderlists)
                {
                    GoodsEntity goods = dbc.GetAll<GoodsEntity>().SingleOrDefault(g => g.Id == orderlist.GoodsId);

                    totalAmount = totalAmount + orderlist.TotalFee;

                    if (goods == null)
                    {
                        continue;
                    }

                    if (!goods.IsPutaway)
                    {
                        return -5;
                    }

                    if (goods.Inventory < orderlist.Number)
                    {
                        return -3;
                    }

                    BonusRatioEntity bonusRatio = dbc.GetAll<BonusRatioEntity>().SingleOrDefault(b => b.GoodsId == goods.Id);
                    decimal one = 0;
                    decimal two = 0;
                    decimal three = 0;

                    long journalTypeId = dbc.GetEntityId<IdNameEntity>(i => i.Name == "佣金收入");

                    UserEntity oneer = dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == user.RecommendId);
                    if (oneer != null && oneer.RecommendPath != "0")
                    {
                        //if (oneer.Level.Name == "普通会员" && bonusRatio != null)
                        //{
                        //    one = bonusRatio.CommonOne / 100;
                        //}
                        //else if (oneer.Level.Name == "黄金会员" && bonusRatio != null)
                        //{
                        //    one = bonusRatio.GoldOne / 100;
                        //}
                        //else if (oneer.Level.Name == "铂金会员" && bonusRatio != null)
                        //{
                        //    one = bonusRatio.PlatinumOne / 100;
                        //}

                        oneer.FrozenAmount = oneer.FrozenAmount + orderlist.TotalFee * one;
                        //oneer.BonusAmount = oneer.BonusAmount + orderlist.TotalFee * one;

                        JournalEntity journal1 = new JournalEntity();
                        journal1.UserId = oneer.Id;
                        //journal1.BalanceAmount = oneer.Amount;
                        journal1.InAmount = orderlist.TotalFee * one;
                        journal1.Remark = "商品佣金收入";
                        journal1.JournalTypeId = 1;
                        journal1.OrderCode = order.Code;
                        journal1.GoodsId = goods.Id;
                        journal1.IsEnabled = false;
                        dbc.Journals.Add(journal1);

                        UserEntity twoer = dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == oneer.RecommendId);
                        if (twoer != null && twoer.RecommendPath != "0")
                        {
                            //if (twoer.Level.Name == "普通会员" && bonusRatio != null)
                            //{
                            //    two = bonusRatio.CommonTwo / 100;
                            //}
                            //else if (twoer.Level.Name == "黄金会员" && bonusRatio != null)
                            //{
                            //    two = bonusRatio.GoldTwo / 100;
                            //}
                            //else if (twoer.Level.Name == "铂金会员" && bonusRatio != null)
                            //{
                            //    two = bonusRatio.PlatinumTwo / 100;
                            //}

                            twoer.FrozenAmount = twoer.FrozenAmount + orderlist.TotalFee * two;
                            //twoer.BonusAmount = twoer.BonusAmount + orderlist.TotalFee * two;

                            JournalEntity journal2 = new JournalEntity();
                            journal2.UserId = twoer.Id;
                            //journal2.BalanceAmount = twoer.Amount;
                            journal2.InAmount = orderlist.TotalFee * two;
                            journal2.Remark = "商品佣金收入";
                            journal2.JournalTypeId = 1;
                            journal2.OrderCode = order.Code;
                            journal2.GoodsId = goods.Id;
                            journal2.IsEnabled = false;
                            dbc.Journals.Add(journal2);

                            UserEntity threer = dbc.GetAll<UserEntity>().SingleOrDefault(u => u.Id == twoer.RecommendId);
                            if (threer != null && threer.RecommendPath != "0")
                            {
                                //if (threer.Level.Name == "普通会员" && bonusRatio != null)
                                //{
                                //    three = bonusRatio.CommonThree / 100;
                                //}
                                //else if (threer.Level.Name == "黄金会员" && bonusRatio != null)
                                //{
                                //    three = bonusRatio.GoldThree / 100;
                                //}
                                //else if (threer.Level.Name == "铂金会员" && bonusRatio != null)
                                //{
                                //    three = bonusRatio.PlatinumThree / 100;
                                //}

                                threer.FrozenAmount = threer.FrozenAmount + orderlist.TotalFee * three;
                                //threer.BonusAmount = threer.BonusAmount + orderlist.TotalFee * three;

                                JournalEntity journal3 = new JournalEntity();
                                journal3.UserId = threer.Id;
                                //journal3.BalanceAmount = threer.Amount;
                                journal3.InAmount = orderlist.TotalFee * three;
                                journal3.Remark = "商品佣金收入";
                                journal3.JournalTypeId = 1;
                                journal3.OrderCode = order.Code;
                                journal3.GoodsId = goods.Id;
                                journal3.IsEnabled = false;
                                dbc.Journals.Add(journal3);
                            }
                        }
                    }
                    goods.Inventory = goods.Inventory - orderlist.Number;
                    goods.SaleNum = goods.SaleNum + orderlist.Number;
                }
                decimal up1 = 0;
                decimal up2 = 0;
                decimal up3 = 0;

                string upSetting1 = dbc.GetStringProperty<SettingEntity>(i => i.Name == "普通会员→黄金会员", i => i.Param);
                string upSetting2 = dbc.GetStringProperty<SettingEntity>(i => i.Name == "普通会员→→铂金会员", i => i.Param);
                string upSetting3 = dbc.GetStringProperty<SettingEntity>(i => i.Name == "黄金会员→铂金会员", i => i.Param);

                decimal.TryParse(upSetting1, out up1);
                decimal.TryParse(upSetting2, out up2);
                decimal.TryParse(upSetting3, out up3);

                int level1 = 1;
                int level2 = 2;
                int level3 = 3;

                int levelId = user.LevelId;
                int upLevelId = 1;
                user.BuyAmount = user.BuyAmount + order.Amount;

                order.PayTime = DateTime.Now;
                order.PayTypeId = (int)PayTypeEnum.微信;
                order.OrderStateId = (int)OrderStateEnum.待发货;
                if (order.Deliver == "无需物流")
                {
                    order.OrderStateId = (int)OrderStateEnum.已完成;
                }

                if (levelId == level1)
                {
                    if (order.Amount >= up1 && order.Amount < up2)
                    {
                        upLevelId = level2;
                    }
                    else if (order.Amount >= up2)
                    {
                        upLevelId = level3;
                    }
                }
                else if (levelId == level2)
                {
                    if (order.Amount >= up3)
                    {
                        upLevelId = level3;
                    }
                }

                JournalEntity journal = new JournalEntity();
                journal.UserId = user.Id;
                journal.BalanceAmount = user.Amount;
                journal.OutAmount = order.Amount;
                journal.Remark = "微信支付购买商品";
                journal.JournalTypeId = 1;
                //journal.LevelId = upLevelId;
                journal.OrderCode = order.Code;
                dbc.Journals.Add(journal);

                dbc.SaveChanges();
                log.DebugFormat("微信支付后订单状态：{0}", order.OrderStateId);
                return 1;
            }
        }

        public async Task<long> WeChatPayAsync(string code)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                using (var scope = dbc.Database.BeginTransaction())
                {
                    try
                    {
                        OrderEntity order = await dbc.GetAll<OrderEntity>().SingleOrDefaultAsync(o => o.Code == code);
                        if (order == null)
                        {
                            return -1;
                        }
                        if (order.OrderStateId != (int)OrderStateEnum.待付款)
                        {
                            return -4;
                        }
                        UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == order.BuyerId);
                        if (user == null)
                        {
                            return -2;
                        }
                        if (!user.IsEnabled)
                        {
                            return -6;
                        }

                        var orderlists = dbc.GetAll<OrderListEntity>().Where(o => o.OrderId == order.Id).ToList();
                        decimal totalAmount = 0;
                        foreach (var orderlist in orderlists)
                        {
                            GoodsEntity goods = await dbc.GetAll<GoodsEntity>().SingleOrDefaultAsync(g => g.Id == orderlist.GoodsId);
                            totalAmount = totalAmount + orderlist.TotalFee;

                            if (goods == null)
                            {
                                continue;
                            }

                            if (!goods.IsPutaway)
                            {
                                return -5;
                            }

                            if (goods.Inventory < orderlist.Number)
                            {
                                return -3;
                            }

                            //商品销量、库存
                            goods.Inventory = goods.Inventory - orderlist.Number;
                            goods.SaleNum = goods.SaleNum + orderlist.Number;
                        }
                        //user.Amount = user.Amount - order.Amount;
                        user.BuyAmount = user.BuyAmount + order.Amount;
                        user.OrderCount = user.OrderCount + 1;

                        order.PayTime = DateTime.Now;
                        order.PayTypeId = (int)PayTypeEnum.微信;
                        order.OrderStateId = (int)OrderStateEnum.待发货;

                        JournalEntity journal = new JournalEntity();
                        journal.UserId = user.Id;
                        journal.BalanceAmount = user.Amount;
                        journal.OutAmount = order.Amount;
                        journal.Remark = "用户(" + user.Mobile + ")购买商品";
                        journal.JournalTypeId = (int)JournalTypeEnum.购买商品;
                        journal.OrderCode = order.Code;
                        journal.GoodsId = order.Id;//来至订单ID
                        journal.CurrencyType = 1;//币种,只有rmb
                        dbc.Journals.Add(journal);
                        await dbc.SaveChangesAsync();

                        if (user.LevelId == (int)LevelEnum.用户)
                        {
                            //购买用户升级
                            await UserLevelUpAsync(dbc, user, order);
                        }
                        else
                        {
                            if (user.BuyType == (int)BuyTypeEnum.首次购买)
                            {
                                user.BuyType = (int)BuyTypeEnum.再次购买;
                                await dbc.SaveChangesAsync();
                            }
                        }

                        long recommendId = user.RecommendId;
                        while (recommendId > 0)
                        {
                            UserEntity recUser = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == recommendId);

                            switch (recUser.LevelId)
                            {
                                case (int)LevelEnum.会员: await MemberLevelUpAsync(dbc, recUser); break;
                                case (int)LevelEnum.银卡会员: await SilverMemberLevelUpAsync(dbc, recUser); break;
                                case (int)LevelEnum.黄金会员: await GoldMemberLevelUpAsync(dbc, recUser); break;
                                case (int)LevelEnum.钻石会员: await DiamondMemberLevelUpAsync(dbc, recUser); break;
                                case (int)LevelEnum.至尊会员: await ExtremeMemberLevelUpAsync(dbc, recUser); break;
                                case (int)LevelEnum.皇冠会员: await CrownMemberLevelUpAsync(dbc, recUser); break;
                            }
                            recommendId = recUser.RecommendId;
                        }

                        //发放奖励
                        await CalcAwardRecommendAsync(dbc, user, order);
                        log.DebugFormat("微信支付后订单状态：{0}", order.OrderStateId);
                        scope.Commit();
                        return order.Id;
                    }
                    catch(Exception ex)
                    {
                        log.ErrorFormat("WeChatPay:{0}", ex.ToString());
                        scope.Rollback();
                        return -8;
                    }
                }                    
            }
        }

        public async Task<CalcAmountResult> CalcCount()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                CalcAmountResult res = new CalcAmountResult();
                var users = dbc.GetAll<UserEntity>().AsNoTracking();
                var takeCash = dbc.GetAll<TakeCashEntity>().AsNoTracking().Where(t => t.StateId==(int)TakeCashStateEnum.已结款);
                res.TotalAmount = users.Any() ? await users.SumAsync(u => u.Amount) : 0;
                res.TotalTakeCash = takeCash.Any() ? await takeCash.SumAsync(u => u.Amount) : 0;
                res.TotalBuyAmount = users.Any() ? await users.SumAsync(u => u.BuyAmount) : 0;
                return res;
            }
        }

        public async Task<decimal> GetTeamBuyAmountAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //RecommendEntity recommend = await dbc.GetAll<RecommendEntity>().AsNoTracking().Where(u => u.IsNull == false).SingleOrDefaultAsync(r => r.UserId == id);
                //if(recommend==null)
                //{
                //    return 0;
                //}
                //var recommends = dbc.GetAll<RecommendEntity>().AsNoTracking().Where(u => u.IsNull == false);

                //if (recommend.RecommendMobile == "superhero" && recommend.RecommendGenera == 1)
                //{
                //    recommends = recommends.Where(a => a.RecommendId == id ||
                // (a.RecommendPath.Contains(id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                // (a.RecommendPath.Contains(id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
                //}
                //else
                //{
                //    recommends = recommends.Where(a => a.RecommendId == id ||
                // (a.RecommendPath.Contains("-" + id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                // (a.RecommendPath.Contains("-" + id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
                //}
                //if(recommends.LongCount()<=0)
                //{
                //    return 0;
                //}
                //return await recommends.Include(r => r.User).SumAsync(r => r.User.BuyAmount);、
                return 0;
            }
        }

        public decimal GetTeamBuyAmount(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                //RecommendEntity recommend = dbc.GetAll<RecommendEntity>().AsNoTracking().Where(u => u.IsNull == false).SingleOrDefault(r => r.UserId == id);
                //if (recommend == null)
                //{
                //    return 0;
                //}
                //var recommends = dbc.GetAll<RecommendEntity>().AsNoTracking().Where(u => u.IsNull == false);

                //if (recommend.RecommendMobile == "superhero" && recommend.RecommendGenera == 1)
                //{
                //    recommends = recommends.Where(a => a.RecommendId == id ||
                // (a.RecommendPath.Contains(id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                // (a.RecommendPath.Contains(id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
                //}
                //else
                //{
                //    recommends = recommends.Where(a => a.RecommendId == id ||
                // (a.RecommendPath.Contains("-" + id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 2) ||
                // (a.RecommendPath.Contains("-" + id.ToString() + "-") && a.RecommendGenera == recommend.RecommendGenera + 3));
                //}
                //if (recommends.LongCount() <= 0)
                //{
                //    return 0;
                //}
                //return recommends.Include(r => r.User).Sum(r => r.User.BuyAmount);
                return 0;
            }
        }

        public async Task<CalcLevelDataModel[]> GetLevelDataAsync()
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var res = MyEnumHelper.GetEnumList<LevelEnum>();
                List<CalcLevelDataModel> list = new List<CalcLevelDataModel>();
                foreach (var item in res)
                {
                    CalcLevelDataModel model = new CalcLevelDataModel();
                    model.Name = item.name;
                    model.Count = await dbc.GetAll<UserEntity>().AsNoTracking().CountAsync(a=>a.LevelId==item.id);
                    list.Add(model);
                }
                return list.ToArray();
            }
        }

        public async Task<decimal> GetAreaScoreAsync(string sheng, string shi, string qu)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var entities = dbc.GetAll<UserEntity>();
                string keyword = string.Empty;
                if(!string.IsNullOrEmpty(sheng))
                {
                    keyword = sheng;
                    if(!string.IsNullOrEmpty(shi))
                    {
                        keyword = keyword + shi;
                        if(!string.IsNullOrEmpty(qu))
                        {
                            keyword = keyword + qu;
                        }
                    }
                }
                if(!string.IsNullOrEmpty(keyword))
                {
                    entities = entities.Where(a => a.Address.Contains(keyword));
                }
                if(!(await entities.AnyAsync()))
                {
                    return 0;
                }
                return await entities.SumAsync(a => a.BuyAmount);
            }
        }

        public async Task<UserDTO> GetModelAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserEntity entity = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Id == id);
                if (entity == null)
                {
                    return null;
                }
                return ToDTO(entity);
            }
        }

        public async Task<bool> CheckIsDiscountAsync(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                var user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return false;
                }
                if (user.LevelId > (int)LevelEnum.用户 && user.BuyType == (int)BuyTypeEnum.再次购买)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<string> GetMobileById(long id)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                string mobile = await dbc.GetStringPropertyAsync<UserEntity>(u => u.Id == id, u => u.Mobile);
                if (mobile == null)
                {
                    return "";
                }
                return mobile;
            }
        }

        public async Task<UserDTO> GetModelByMobileAsync(string mobile)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var user = await dbc.GetAll<UserEntity>().AsNoTracking().SingleOrDefaultAsync(u => u.Mobile == mobile);
                if (user == null)
                {
                    return null;
                }
                return ToDTO(user);
            }
        }

        public async Task<UserSearchResult> GetModelListAsync(int? levelId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserSearchResult result = new UserSearchResult();
                var users = dbc.GetAll<UserEntity>().AsNoTracking();

                if (levelId != null)
                {
                    users = users.Where(a => a.LevelId == levelId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    users = users.Where(a => a.Mobile.Contains(keyword) || a.UserCode.Contains(keyword) || a.NickName.Contains(keyword));
                }
                if (startTime != null)
                {
                    users = users.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    users = users.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.PageCount = (int)Math.Ceiling((await users.LongCountAsync()) * 1.0f / pageSize);
                var userResult = await users.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Users = userResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
        public async Task<UserTeamSearchResult> GetModelTeamListAsync(string mobile, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTeamSearchResult result = new UserTeamSearchResult();
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Mobile == mobile);
                var recommends = dbc.GetAll<UserEntity>().AsNoTracking();
                string path = user.Id.ToString();
                if (teamLevel != null)
                {
                    if (user.UserCode == "system")
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == user.Id);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains(path + "-") && a.RecommendGenera == user.RecommendGenera + 2);
                        }
                    }
                    else
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == user.Id);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains("-" + path + "-") && a.RecommendGenera == user.RecommendGenera + 2);
                        }
                    }
                }
                else
                {
                    if (user.UserCode == "system")
                    {
                        recommends = recommends.Where(a => a.RecommendId == user.Id ||
                     (a.RecommendPath.Contains(path + "-") && a.RecommendGenera == user.RecommendGenera + 2));
                    }
                    else
                    {
                        recommends = recommends.Where(a => a.RecommendId == user.Id ||
                     (a.RecommendPath.Contains("-" + path + "-") && a.RecommendGenera == user.RecommendGenera + 2));
                    }
                }
                if (keyword != null)
                {
                    recommends = recommends.Where(a => a.Mobile.Contains(keyword) || a.UserCode.Contains(keyword) || a.NickName.Contains(keyword));
                }
                if (startTime != null)
                {
                    recommends = recommends.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    recommends = recommends.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.TeamLeader = ToDTO(user);
                result.TotalCount = recommends.LongCount();
                result.PageCount = (int)Math.Ceiling(recommends.LongCount() * 1.0f / pageSize);
                var userResult = await recommends.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Members = userResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
        public async Task<UserTeamSearchResult> GetModelTeamListAsync(long userId, long? teamLevel, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                UserTeamSearchResult result = new UserTeamSearchResult();
                UserEntity user = await dbc.GetAll<UserEntity>().SingleOrDefaultAsync(u => u.Id == userId);
                var recommends = dbc.GetAll<UserEntity>().AsNoTracking();
                string path = user.Id.ToString();
                if (teamLevel != null)
                {
                    if (user.UserCode=="system")
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == user.Id);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains(path + "-") && a.RecommendGenera == user.RecommendGenera + 2);
                        }
                    }
                    else
                    {
                        if (teamLevel == 1)
                        {
                            recommends = recommends.Where(a => a.RecommendId == user.Id);
                        }
                        else if (teamLevel == 2)
                        {
                            recommends = recommends.Where(a => a.RecommendPath.Contains("-" + path + "-") && a.RecommendGenera == user.RecommendGenera + 2);
                        }
                    }
                }
                else
                {
                    if (user.UserCode == "system")
                    {
                        recommends = recommends.Where(a => a.RecommendId == user.Id ||
                     (a.RecommendPath.Contains(path + "-") && a.RecommendGenera == user.RecommendGenera + 2));
                    }
                    else
                    {
                        recommends = recommends.Where(a => a.RecommendId == userId ||
                     (a.RecommendPath.Contains("-" + path + "-") && a.RecommendGenera == user.RecommendGenera + 2));
                    }
                }
                if (keyword != null)
                {
                    recommends = recommends.Where(a => a.Mobile.Contains(keyword) || a.UserCode.Contains(keyword) || a.NickName.Contains(keyword));
                }
                if (startTime != null)
                {
                    recommends = recommends.Where(a => a.CreateTime >= startTime);
                }
                if (endTime != null)
                {
                    recommends = recommends.Where(a => SqlFunctions.DateDiff("day", endTime, a.CreateTime) <= 0);
                }
                result.TotalCount = recommends.LongCount();
                result.PageCount = (int)Math.Ceiling(recommends.LongCount() * 1.0f / pageSize);
                var userResult = await recommends.OrderByDescending(a => a.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                result.Members = userResult.Select(a => ToDTO(a)).ToArray();
                return result;
            }
        }
    }
}
