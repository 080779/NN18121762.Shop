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
    /// 退货管理
    /// </summary>
    public class ReturnController : Controller
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
        [AdminLog("退货管理", "查看退货管理列表")]
        public async Task<ActionResult> List(long? auditStatusId, string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            //long orderStateId = await idNameService.GetIdByNameAsync("退货中");
            var result = await orderService.GetReturnModelListAsync(null, null, auditStatusId, keyword, startTime, endTime, pageIndex, pageSize);
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
        [AdminLog("订单管理", "查看订单管理明细")]
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
        [Permission("退货管理_退货审核")]
        [AdminLog("退货管理", "退货管理审核")]
        public async Task<ActionResult> Audit(long id)
        {
            long res = await orderService.ReturnAuditAsync(id, Convert.ToInt64(Session["Platform_AdminUserId"]));
            if (res <= 0)
            {
                if(res==-2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "订单中没有要退货的商品" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "退货审核失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "退货审核成功" });
        }

        [HttpPost]
        [Permission("退货管理_标记退货成功")]
        [AdminLog("退货管理", "退货管理确认退货")]
        public async Task<ActionResult> Confirm(long id)
        {
            long res = await orderService.ReturnAsync(id);
            if(res<=0)
            {
                if(res==-2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "订单中没有要退货的商品" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "确认退货完成失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "确认退货完成成功" });
        }
    }
}