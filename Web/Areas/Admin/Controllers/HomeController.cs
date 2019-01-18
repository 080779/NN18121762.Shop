using IMS.Common;
using IMS.IService;
using IMS.Web.Areas.Admin.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IAdminService adminService { get; set; }
        public ISettingService settingService { get; set; }
        public IPermissionTypeService permissionTypeService { get; set; }
        public IOrderService orderService { get; set; }
        public async Task<ActionResult> Index()
        {
            long userId = Convert.ToInt64(Session["Platform_AdminUserId"]);
            HomeIndexViewModel model = new HomeIndexViewModel();
            model.PermissionTypes = await permissionTypeService.GetModelList();
            model.Mobile = await adminService.GetMobileAsync(userId);
            return View(model);
        }
        public async Task<ActionResult> Home()
        {
            var res = await orderService.GetDataAsync();
            return View(res);
        }
        public async Task<ActionResult> Get()
        {
            var list = await orderService.GetTotalAmountAsync();
            return Json(new AjaxResult { Status=1,Data=list});
        }
        public ActionResult Permission(string msg)
        {
            return View((object)msg);
        }
    }
}