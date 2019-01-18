using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Goods;
using IMS.Web.Models.GoodsCar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class GoodsCarController : ApiController
    {
        private string doMain = System.Configuration.ConfigurationManager.AppSettings["DoMain"];
        public IGoodsCarService goodsCarService { get; set; }
        public ISettingService settingService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List()
        {
            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            GoodsCarSearchResult result = await goodsCarService.GetModelListAsync(user.Id, null, null, null, 1, 100);
            GoodsCarListApiModel model = new GoodsCarListApiModel();
            model.goodsCars = result.GoodsCars.Select(g => new GoodsCarModel { id = g.Id, inventory=g.Inventory, price = g.Price, goodsId = g.GoodsId, goodsName = g.Name, realityPrice = g.RealityPrice, number = g.Number, goodsAmount = g.GoodsAmount, imgUrl = parm + g.ImgUrl, isSelected = g.IsSelected }).ToList();
            model.totalAmount = result.TotalAmount;
            return new ApiResult { status = 1, data = model };
        }
        [HttpPost]
        public async Task<ApiResult> SelectList()
        {
            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            GoodsCarSearchResult result = await goodsCarService.GetModelListAsync(user.Id,true, null, null, null, 1, 100);
            GoodsCarListApiModel model = new GoodsCarListApiModel();
            model.goodsCars = result.GoodsCars.Select(g => new GoodsCarModel { id = g.Id, inventory = g.Inventory, price = g.Price, goodsId = g.GoodsId, goodsName = g.Name, realityPrice = g.RealityPrice, number = g.Number, goodsAmount = g.GoodsAmount, imgUrl = parm + g.ImgUrl, isSelected = g.IsSelected }).ToList();
            model.totalAmount = result.TotalAmount;
            return new ApiResult { status = 1, data = model };
        }
        [HttpPost]
        public async Task<ApiResult> Add(GoodsCarAddModel model)
        {
            if (model.Id <= 0)
            {
                return new ApiResult { status = 0, msg = "商品id错误" };
            }
            //if (model.Number <= 0)
            //{
            //    return new ApiResult { status = 0, msg = "商品数量错误" };
            //}
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await goodsCarService.AddAsync(user.Id, model.Id, model.Number);
            if (id <= 0)
            {
                if (id == -1)
                {
                    return new ApiResult { status = 0, msg = "商品不存在" };
                }
                if (id == -2)
                {
                    return new ApiResult { status = 0, msg = "商品库存不足" };
                }
                if (id == -3)
                {
                    return new ApiResult { status = 0, msg = "购物车减商品数量，只能传-1" };
                }
                if (id == -4)
                {
                    return new ApiResult { status = 0, msg = "购物车减商品数量,商品数量不能小于1" };
                }
                if (id == -5)
                {
                    return new ApiResult { status = 0, msg = "添加购物车的商品数量不能小于1" };
                }
                return new ApiResult { status = 0, msg = "添加商品到购物车失败" };
            }
            return new ApiResult { status = 1, msg = "添加商品到购物车成功", data = id };
        }
        [HttpPost]
        public async Task<ApiResult> Edit(GoodsCarEditModel model)
        {
            if (model.Id <= 0)
            {
                return new ApiResult { status = 0, msg = "购物车id错误" };
            }
            if (model.Number <= 0)
            {
                return new ApiResult { status = 0, msg = "商品数量错误" };
            }
            long id = await goodsCarService.UpdateAsync(model.Id, model.Number, model.IsSelected);
            if (id<=0)
            {
                if(id==-1)
                {
                    return new ApiResult { status = 0, msg = "购物车商品记录不存在" };
                }
                if (id == -2)
                {
                    return new ApiResult { status = 0, msg = "商品不存在" };
                }
                if (id == -3)
                {
                    return new ApiResult { status = 0, msg = "商品库存不足" };
                }
                return new ApiResult { status = 0, msg = "更新购物车商品失败" };
            }
            return new ApiResult { status = 1, msg = "更新购物车商品成功" };
        }

        [HttpPost]
        public async Task<ApiResult> Del(GoodsCarDelModel model)
        {
            if (model.Id <= 0)
            {
                return new ApiResult { status = 0, msg = "购物车id错误" };
            }
            bool flag = await goodsCarService.DeleteAsync(model.Id);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "删除购物车商品失败" };
            }
            return new ApiResult { status = 1, msg = "删除购物车商品成功" };
        }
    }
}