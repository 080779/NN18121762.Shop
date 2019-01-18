using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Deliver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 发货管理
    /// </summary>
    public class DeliverController : Controller
    {
        public IOrderService orderService { get; set; }
        public IIdNameService idNameService { get; set; }
        public IOrderListService orderListService { get; set; }
        private int pageSize = 10;
        //[Permission("日志管理_查看日志")]
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        //[Permission("日志管理_查看日志")]
        [AdminLog("发货管理", "查看收发货列表")]
        public async Task<ActionResult> List(int? orderStateId,string keyword,DateTime? startTime,DateTime? endTime,int pageIndex=1)
        {
            var result = await orderService.GetDeliverModelListAsync(null,orderStateId, keyword, startTime, endTime, pageIndex, pageSize);
            DeliverListViewModel res = new DeliverListViewModel();
            res.Orders = result.Orders;
            res.PageCount = result.PageCount;
            res.OrderStates = MyEnumHelper.GetEnumList<OrderStateEnum>().Where(s=>s.id==(int)OrderStateEnum.待发货 || s.id == (int)OrderStateEnum.已发货).ToArray();
            return Json(new AjaxResult { Status = 1, Data = res });
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
            DeliverDetailViewModel model = new DeliverDetailViewModel();
            model.Order = dto;
            model.OrderList = result.OrderLists;
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        public async Task<ActionResult> GetModel(long id)
        {
            var res = await orderService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = res });
        }
        [Permission("发货管理_标记发货")]
        [AdminLog("发货管理", "标记发货")]
        public async Task<ActionResult> Edit(long id,string deliver,string deliverName,string deliverCode)
        {
            //if(string.IsNullOrEmpty(deliver))
            //{
            //    return Json(new AjaxResult { Status = 0, Msg = "请选择物流方式" });
            //}
            long flag = await orderService.UpdateDeliverStateAsync(id,deliver, deliverName, deliverCode);
            if(flag<=0)
            {
                if (flag == -1)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "订单不存在" });
                }
                if(flag==-2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "订单退单中或退货中" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "标记发货失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "标记发货成功" });
        }
    }
}