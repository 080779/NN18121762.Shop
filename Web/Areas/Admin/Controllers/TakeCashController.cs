using IMS.Common;
using IMS.Common.Enums;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.TakeCash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class TakeCashController : Controller
    {
        public ITakeCashService takeCashService { get; set; }
        public IIdNameService idNameService { get; set; }
        private int pageSize = 10;
        //[Permission("提现管理_查看记录")]
        public ActionResult List()
        {
            return View();
        }
        //[Permission("积分管理_积分管理")]
        [AdminLog("提现管理", "查看提现管理列表")]
        [HttpPost]
        public async Task<ActionResult> List(long? stateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            TakeCashSearchResult result = await takeCashService.GetModelListAsync(null,stateId, keyword, startTime, endTime, pageIndex, pageSize);
            TakeCashListViewModel model = new TakeCashListViewModel();
            model.TakeCashes = result.TakeCashes;
            model.PageCount = result.PageCount;
            model.States = MyEnumHelper.GetEnumList<TakeCashStateEnum>();
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [HttpPost]
        [AdminLog("提现管理", "确认结款")]
        [Permission("提现管理_标记结款")]
        public async Task<ActionResult> Confirm(long id)
        {
            long res = await takeCashService.ConfirmAsync(id, Convert.ToInt64(Session["Platform_AdminUserId"]));
            if(res<=0)
            {
                if(res==-3)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "账户余额不足,结款失败" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "结款失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "结款成功" });
        }

        [HttpPost]
        [AdminLog("提现管理", "取消结款")]
        [Permission("提现管理_取消结款")]
        public async Task<ActionResult> Cancel(long id,string description)
        {
            long res = await takeCashService.CancelAsync(id, description, Convert.ToInt64(Session["Platform_AdminUserId"]));
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "取消结款失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "取消结款成功" });
        }
    }
}