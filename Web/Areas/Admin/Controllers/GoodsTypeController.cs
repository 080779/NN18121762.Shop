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
    public class GoodsTypeController : Controller
    {
        private int pageSize = 10;
        public IGoodsTypeService goodsTypeService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("商品分类", "查看商品分类列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            GoodsTypeSearchResult result= await goodsTypeService.GetModelListAsync(keyword,startTime,endTime,pageIndex,pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        [AdminLog("商品分类", "添加商品分类")]
        [Permission("商品分类_新增分类")]
        public async Task<ActionResult> Add(string name, string imgFile, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品类别名不能为空" });
            }
            if (string.IsNullOrEmpty(imgFile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片图片必须上传" });
            }
            string res;
            if (!ImageHelper.SaveBase64(imgFile, out res))
            {
                return Json(new AjaxResult { Status = 0, Msg = res });
            }
            long id = await goodsTypeService.AddAsync(name, res, description);
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加商品类别失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加商品类别成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            GoodsTypeDTO model = await goodsTypeService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [AdminLog("商品分类", "编辑商品分类")]
        [Permission("商品分类_修改分类")]
        public async Task<ActionResult> Edit(long id, string name, string imgFile, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "商品类别名不能为空" });
            }
            if (string.IsNullOrEmpty(imgFile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片图片必须上传" });
            }
            bool flag = true;
            if (imgFile.Contains("upload/"))
            {
                flag = await goodsTypeService.UpdateAsync(id, name, imgFile, description);
            }
            else
            {
                string res;
                if (!ImageHelper.SaveBase64(imgFile, out res))
                {
                    return Json(new AjaxResult { Status = 0, Msg = res });
                }
                flag = await goodsTypeService.UpdateAsync(id, name, res, description);
            }
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "编辑商品类别失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑商品类别成功" });
        }
        [AdminLog("商品分类", "删除商品分类")]
        [Permission("商品分类_删除分类")]
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await goodsTypeService.DeleteAsync(id);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除商品类别失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除商品类别成功" });
        }
    }
}