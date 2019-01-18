using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class SettingController : Controller
    {
        public ISettingService settingService { get; set; }
        //[Permission("日志管理_查看日志")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[Permission("日志管理_查看日志")]
        [AdminLog("系统设置", "查看系统设置")]
        public async Task<ActionResult> List(bool flag=true)
        {
            var model = await settingService.GetModelListIsEnableAsync();
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [HttpPost]
        [AdminLog("系统设置", "编辑系统设置")]
        [Permission("系统设置_系统设置")]
        public async Task<ActionResult> Edit(long id, string parm)
        {
            bool flag = await settingService.EditAsync(id, parm);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "更新失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "更新成功" });
        }
        [HttpPost]
        [AdminLog("系统设置", "编辑系统设置")]
        [Permission("系统设置_系统设置")]
        public async Task<ActionResult> EditAll(SettingSetDTO[] settings)
        {
            bool flag = await settingService.EditAsync(settings);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "更新失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "更新成功" });
        }
    }
}