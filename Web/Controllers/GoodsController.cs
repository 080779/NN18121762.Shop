using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    [AllowAnonymous]
    public class GoodsController : ApiController
    {
        private string doMain = System.Configuration.ConfigurationManager.AppSettings["DoMain"];
        public IGoodsService goodsService { get; set; }
        public IGoodsTypeService goodsTypeService { get; set; }
        public IGoodsImgService goodsImgService { get; set; }
        public IGoodsAreaService goodsAreaService { get; set; }
        public ISettingService settingService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List(GoodsListModel model)
        {
            string parm = doMain;
            GoodsSearchResult result = await goodsService.FrontModelListAsync(model.IsHotSale, model.TypeId, model.SecondTypeId, null, null, null, model.PageIndex, model.PageSize);
            List<SearchResultModel> lists;
            lists = result.Goods.Select(g => new SearchResultModel { id = g.Id,inventory=g.Inventory, name = g.Name, realityPrice = g.RealityPrice, saleNum = g.SaleNum, imgUrl=parm+goodsImgService.GetFirstImg(g.Id)}).ToList();
            GoodsSearchApiModel apiModel = new GoodsSearchApiModel();
            apiModel.goods = lists;
            apiModel.pageCount = result.PageCount;
            return new ApiResult { status = 1, data = apiModel };
        }
        [HttpPost]
        public async Task<ApiResult> HotSales(GoodsHotSalesModel model)
        {
            string parm = doMain;
            GoodsSearchResult result = await goodsService.FrontModelListAsync(true, null,null,null, null, null, model.PageIndex, model.PageSize);
            //goodsImgService
            List<SearchResultModel> lists;
            lists = result.Goods.Select(g => new SearchResultModel { id = g.Id, inventory = g.Inventory, name = g.Name, price=g.Price, realityPrice = g.RealityPrice, saleNum = g.SaleNum,imgUrl= parm + goodsImgService.GetFirstImg(g.Id) }).ToList();
            GoodsSearchApiModel apiModel = new GoodsSearchApiModel();
            apiModel.goods = lists;
            apiModel.pageCount = result.PageCount;
            return new ApiResult { status = 1, data = apiModel };
        }
        [HttpPost]
        public async Task<ApiResult> Search(GoodsSearchModel model)
        {
            string parm = doMain;
            GoodsSearchResult result= await goodsService.SearchAsync(model.KeyWord, null, null, model.PageIndex, model.PageSize);
            List<SearchResultModel> lists;
            lists = result.Goods.Select(g => new SearchResultModel { id = g.Id, inventory = g.Inventory, name = g.Name, price = g.Price, realityPrice = g.RealityPrice, saleNum = g.SaleNum, imgUrl = parm + goodsImgService.GetFirstImg(g.Id) }).ToList();
            GoodsSearchApiModel apiModel = new GoodsSearchApiModel();
            apiModel.goods = lists;
            apiModel.pageCount = result.PageCount;
            return new ApiResult { status = 1,data=apiModel };
        }
        [HttpPost]
        public async Task<ApiResult> Detail(GoodsDetailModel model)
        {
            string parm = doMain;
            GoodsDTO dto= await goodsService.GetModelAsync(model.Id);
            GoodsImgSearchResult result = await goodsImgService.GetModelListAsync(dto.Id,null, null, null, 1, 100);
            GoodsDetailApiModel apiModel = new GoodsDetailApiModel();
            apiModel.goodsImgs = result.GoodsImgs.Select(g => new GoodsImg { id = g.Id, imgUrl = parm + g.ImgUrl }).ToList();
            if (dto!=null)
            {
                apiModel.id = dto.Id;
                apiModel.description = dto.Description.Replace("/upload/",parm+ "/upload/");
                apiModel.inventory = dto.Inventory;
                apiModel.name = dto.Name;
                apiModel.price = dto.Price;
                apiModel.realityPrice = dto.RealityPrice;
                apiModel.saleNum = dto.SaleNum;
            }
            return new ApiResult { status = 1, data = apiModel };
        }
    }
}