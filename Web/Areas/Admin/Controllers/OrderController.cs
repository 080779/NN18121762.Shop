using IMS.Common;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IMS.Web.Areas.Admin.Models.Order;
using IMS.DTO;
using SDMS.Common;
using IMS.Web.App_Start.Filter;
using IMS.Common.Enums;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private int pageSize = 10;
        public IOrderService orderService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IOrderListService orderListService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("订单管理", "查看订单管理列表")]
        public async Task<ActionResult> List(int? orderStateId, string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await orderService.GetModelListAsync(null, orderStateId, null, keyword, startTime, endTime, pageIndex, pageSize);
            OrderListViewModel model = new OrderListViewModel();
            model.Orders = result.Orders; 
            model.PageCount = result.PageCount;
            model.OrderStates = MyEnumHelper.GetEnumList<OrderStateEnum>();
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
            OrderDTO dto= await orderService.GetModelAsync(id);
            OrderListSearchResult result = await orderListService.GetModelListAsync(dto.Id, null, null, null, 1, 100);
            OrderDetailViewModel model = new OrderDetailViewModel();
            model.Order = dto;
            model.OrderList = result.OrderLists;
            model.GoodsAmount = result.OrderLists.Sum(o => o.TotalFee);
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [AdminLog("订单管理", "导出订单管理列表")]
        [Permission("订单管理_订单导出")]
        public async Task<ActionResult> ExportExcel()
        {
            var res = await orderService.GetAllAsync();
            OrderExportExcelModel[] result = res.Select(o => new OrderExportExcelModel
            {
                Amount = o.Amount,
                BuyerMobile=o.BuyerMobile,
                BuyerNickName=o.BuyerNickName,
                Code=o.Code,
                CreateTime=o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Id=o.Id,
                OrderStateName=o.OrderStateName,
                PayTypeName=o.PayTypeName,
                ReceiverAddress=o.ReceiverAddress,
                ReceiverMobile=o.ReceiverMobile,
                ReceiverName=o.ReceiverName
            }).ToArray();
            return File(ExcelHelper.ExportExcel<OrderExportExcelModel>(result, "订单"), "application/vnd.ms-excel", "订单.xls");
        }

        [HttpPost]
        [AdminLog("订单管理", "修改收货地址")]
        [Permission("订单管理_修改收货地址")]
        public async Task<ActionResult> EditReceiverInfo(long id, string receiverName, string receiverMobile, string receiverAddress)
        {
            var res = await orderService.EditReceiverInfoAsync(id,receiverName,receiverMobile,receiverAddress);
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Data = "修改失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改成功" });
        }
    }
}