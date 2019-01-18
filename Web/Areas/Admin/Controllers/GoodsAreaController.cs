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
    public class GoodsAreaController : Controller
    {
        private int pageSize = 10;
        public IGoodsAreaService goodsAreaService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("首页产品分区管理", "查看分区列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            GoodsAreaSearchResult result= await goodsAreaService.GetModelListAsync(keyword,startTime,endTime,pageIndex,pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        [AdminLog("首页产品分区管理", "添加分区")]
        [Permission("首页产品分区管理_新增分区")]
        public async Task<ActionResult> Add(string title, string description, string note)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Json(new AjaxResult { Status = 0, Msg = "分区标题称不能为空" });
            }
            if (string.IsNullOrEmpty(description))
            {
                return Json(new AjaxResult { Status = 0, Msg = "分区描述不能为空" });
            }
            long id = await goodsAreaService.AddAsync(title, description, note);
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加首页产品分区失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加首页产品分区成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            GoodsAreaDTO model = await goodsAreaService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [AdminLog("首页产品分区管理", "编辑分区")]
        [Permission("首页产品分区管理_修改分区")]
        public async Task<ActionResult> Edit(long id,string title, string description, string note)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Json(new AjaxResult { Status = 0, Msg = "分区标题称不能为空" });
            }
            if (string.IsNullOrEmpty(description))
            {
                return Json(new AjaxResult { Status = 0, Msg = "分区描述不能为空" });
            }
            bool  flag = await goodsAreaService.UpdateAsync(id,title, description, note);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "编辑首页产品分区失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑首页产品分区成功" });
        }
        [AdminLog("首页产品分区管理", "删除分区")]
        [Permission("首页产品分区管理_删除分区")]
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await goodsAreaService.DeleteAsync(id);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除首页产品分区失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除首页产品分区成功" });
        }
    }
}