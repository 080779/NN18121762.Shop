using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class SlideController : Controller
    {
        private int pageSize = 10;
        public ISlideService slideService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("幻灯片管理", "查看幻灯片管理列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await slideService.GetModelListAsync(keyword, startTime, endTime, pageIndex, pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        [AdminLog("幻灯片管理", "添加幻灯片")]
        [Permission("幻灯片管理_新增幻灯片")]
        public async Task<ActionResult> Add(string name, string url, string imgFile, bool isEnabled)
        {
            if(string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片名称不能为空" });
            }
            //if (string.IsNullOrEmpty(url))
            //{
            //    return Json(new AjaxResult { Status = 0, Msg = "转向连接不能为空" });
            //}
            if (string.IsNullOrEmpty(imgFile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片图片必须上传" });
            }
            string res;
            if(!ImageHelper.SaveBase64(imgFile, out res))
            {
                return Json(new AjaxResult { Status = 0, Msg=res});
            }
            long id=  await slideService.AddAsync(name, url, res, isEnabled);
            if(id<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加幻灯片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加幻灯片成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            SlideDTO model= await slideService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data= model });
        }
        [AdminLog("幻灯片管理", "编辑幻灯片")]
        [Permission("幻灯片管理_修改幻灯片")]
        public async Task<ActionResult> Edit(long id,string name, string url, string imgFile, bool isEnabled)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片名称不能为空" });
            }
            //if (string.IsNullOrEmpty(url))
            //{
            //    return Json(new AjaxResult { Status = 0, Msg = "转向连接不能为空" });
            //}
            if (string.IsNullOrEmpty(imgFile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "幻灯片图片必须上传" });
            }
            bool flag = true;
            if (imgFile.Contains("upload/"))
            {
                flag = await slideService.UpdateAsync(id, name, url, imgFile, isEnabled);
            }
            else
            {
                string res;
                if (!ImageHelper.SaveBase64(imgFile, out res))
                {
                    return Json(new AjaxResult { Status = 0, Msg = res });
                }
                flag = await slideService.UpdateAsync(id, name, url, res, isEnabled);
            }
            
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改幻灯片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改幻灯片成功" });
        }
        [AdminLog("幻灯片管理", "删除幻灯片")]
        [Permission("幻灯片管理_删除幻灯片")]
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await slideService.DeleteAsync(id);
            if(!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除幻灯片失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除幻灯片成功" });
        }
    }
}