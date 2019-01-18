using IMS.Common;
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
    public class AdminLogController : Controller
    {
        public IAdminLogService adminLogService { get; set; }
        private int pageSize = 10;
        //[Permission("日志管理_查看日志")]        
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [Permission("查看日志_查看日志")]
        [AdminLog("查看日志", "查看日志列表")]
        public async Task<ActionResult> List(string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            var result = await adminLogService.GetModelListAsync(keyword, null, startTime, endTime, pageIndex, pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
    }
}