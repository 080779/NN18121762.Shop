using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 退单管理
    /// </summary>
    public class RefundController : Controller
    {
        public IOrderService orderService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IAdminService adminService { get; set; }
        public IOrderListService orderListService { get; set; }
        private int pageSize = 10;
        //[Permission("日志管理_查看日志")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[Permission("日志管理_查看日志")]
        [AdminLog("退单管理", "查看退单管理列表")]
        public async Task<ActionResult> List(long? auditStatusId, string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            var result = await orderService.GetRefundModelListAsync(null, null, auditStatusId, keyword, startTime, endTime, pageIndex, pageSize);
            ReturnListViewModel model = new ReturnListViewModel();
            model.Orders = result.Orders;
            model.PageCount = result.PageCount;
            model.AuditStatus = await idNameService.GetByTypeNameAsync("退货审核状态");
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        public ActionResult Detail(long id)
        {
            return View(id);
        }

        [HttpPost]
        [AdminLog("退单管理", "查看退单管理明细")]
        public async Task<ActionResult> GetDetail(long id)
        {
            OrderDTO dto = await orderService.GetModelAsync(id);
            OrderListSearchResult result = await orderListService.GetModelListAsync(dto.Id, null, null, null, 1, 100);
            ReturnDetailViewModel model = new ReturnDetailViewModel();
            model.Order = dto;
            model.OrderList = result.OrderLists;
            model.GoodsAmount = result.OrderLists.Sum(o => o.TotalFee);
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        [HttpPost]
        [Permission("退单管理_退单审核")]
        [AdminLog("退单管理", "退单管理审核")]
        public async Task<ActionResult> Audit(long id)
        {
            long res = await orderService.RefundAuditAsync(id, Convert.ToInt64(Session["Platform_AdminUserId"]));
            if (res <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "退单审核失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "退单审核成功" });
        }

        [HttpPost]
        [Permission("退单管理_标记退单成功")]
        [AdminLog("退单管理", "退单管理确认退单")]
        public async Task<ActionResult> Confirm(long id)
        {
            long res = await orderService.ReturnOrderAsync(id);
            if(res<=0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "确认退单完成失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "确认退单完成成功" });
        }
    }
}