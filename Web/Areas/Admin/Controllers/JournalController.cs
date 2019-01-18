using IMS.Common;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Journal;
using IMS.Web.Areas.Admin.Models.TakeCash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class JournalController : Controller
    {
        public IJournalService journalService { get; set; }
        public IBonusService bonusService { get; set; }
        public IUserService userService { get; set; }
        public IOrderService orderService { get; set; }
        private int pageSize = 10;
        //[Permission("积分管理_积分管理")]
        public ActionResult List()
        {
            return View();
        }
        [Permission("佣金记录_查看记录")]
        [AdminLog("佣金记录", "查看佣金记录列表")]
        [HttpPost]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //await orderService.AutoConfirmAsync();
            //long journalTypeId = await idNameService.GetIdByNameAsync("佣金收入");
            JournalSearchResult result = await journalService.GetBonusModelListAsync(null, keyword, startTime, endTime, pageIndex,pageSize);
            //CalcAmountResult res = await userService.CalcCount();
            ListViewModel model = new ListViewModel();
            model.List = result.List;
            model.PageCount = result.PageCount;
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [HttpPost]
        [Permission("佣金记录_发放全国分红")]
        [AdminLog("佣金记录", "发放全国分红")]
        public async Task<ActionResult> CalcShareBonus(decimal score)
        {
            if(score<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "加权平分金额必须大于零" });
            }
            var res = await bonusService.CalcShareBonusAsync(score);
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "发放全国分红失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "发放全国分红成功" });
        }
    }
}