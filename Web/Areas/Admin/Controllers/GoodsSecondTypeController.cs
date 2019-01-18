using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Goods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class GoodsSecondTypeController : Controller
    {
        private int pageSize = 10;
        public IGoodsSecondTypeService goodsSecondTypeService { get; set; }
        public ActionResult List(long id)
        {
            return View(id);
        }
        [HttpPost]
        [AdminLog("商品分类", "查看商品二级分类列表")]
        public async Task<ActionResult> List(long id,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            GoodsSecondTypeSearchResult result= await goodsSecondTypeService.GetModelListAsync(id,keyword,startTime,endTime,pageIndex,pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        [AdminLog("商品分类", "添加商品二级分类")]
        public async Task<ActionResult> Add(long id,string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品二级分类名不能为空" });
            }
            long res = await goodsSecondTypeService.AddAsync(id,name, description);
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加商品二级分类失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加商品二级分类成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            GoodsSecondTypeDTO model = await goodsSecondTypeService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [AdminLog("商品分类", "编辑商品二级分类")]
        public async Task<ActionResult> Edit(long id, string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品二级分类名不能为空" });
            }
            bool flag = await goodsSecondTypeService.UpdateAsync(id, name, description);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "编辑商品二级分类失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑商品二级分类成功" });
        }
        [AdminLog("商品分类", "删除商品二级分类")]
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await goodsSecondTypeService.DeleteAsync(id);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除商品二级分类失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除商品二级分类成功" });
        }
    }
}