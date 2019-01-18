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
    public class BonusRatioService : IBonusRatioService
    {
        public async Task<BonusRatio> GetModelAsync(long goodsId)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusRatioEntity entity = await dbc.GetAll<BonusRatioEntity>().SingleOrDefaultAsync(b => b.GoodsId == goodsId);
                if(entity==null)
                {
                    return null;
                }
                return new BonusRatio
                {
                    CommonOne = entity.CommonOne,
                    CommonThree = entity.CommonThree,
                    CommonTwo = entity.CommonTwo,
                    GoldOne = entity.GoldOne,
                    GoldThree = entity.GoldThree,
                    GoldTwo = entity.GoldTwo,
                    GoodsId = entity.GoodsId,
                    PlatinumOne = entity.PlatinumOne,
                    PlatinumThree = entity.PlatinumThree,
                    PlatinumTwo = entity.PlatinumTwo
                };
            }
        }

        public async Task<bool> UpdateAsync(BonusRatio bonusRatio)
        {
            using (MyDbContext dbc = new MyDbContext())
            {
                BonusRatioEntity entity = await dbc.GetAll<BonusRatioEntity>().SingleOrDefaultAsync(b => b.GoodsId == bonusRatio.GoodsId);
                if(entity==null)
                {
                    entity = new BonusRatioEntity();
                    entity.CommonOne = bonusRatio.CommonOne;
                    entity.CommonThree = bonusRatio.CommonThree;
                    entity.CommonTwo = bonusRatio.CommonTwo;
                    entity.GoldOne = bonusRatio.GoldOne;
                    entity.GoldThree = bonusRatio.GoldThree;
                    entity.GoldTwo = bonusRatio.GoldTwo;
                    entity.GoodsId = bonusRatio.GoodsId;
                    entity.PlatinumOne = bonusRatio.PlatinumOne;
                    entity.PlatinumThree = bonusRatio.PlatinumThree;
                    entity.PlatinumTwo = bonusRatio.PlatinumTwo;
                    dbc.BonusRatios.Add(entity);
                }
                else
                {
                    entity.CommonOne = bonusRatio.CommonOne;
                    entity.CommonThree = bonusRatio.CommonThree;
                    entity.CommonTwo = bonusRatio.CommonTwo;
                    entity.GoldOne = bonusRatio.GoldOne;
                    entity.GoldThree = bonusRatio.GoldThree;
                    entity.GoldTwo = bonusRatio.GoldTwo;
                    entity.GoodsId = bonusRatio.GoodsId;
                    entity.PlatinumOne = bonusRatio.PlatinumOne;
                    entity.PlatinumThree = bonusRatio.PlatinumThree;
                    entity.PlatinumTwo = bonusRatio.PlatinumTwo;
                }
                await dbc.SaveChangesAsync();
                return true;
            }
        }
    }
}
