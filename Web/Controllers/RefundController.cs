using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Return;
using IMS.Web.Models.TakeCash;
using IMS.Web.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{    
    public class RefundController : ApiController
    {        
        public IOrderListService orderListService { get; set; }
        public IOrderService orderService { get; set; }
        [HttpPost]
        public async Task<ApiResult> Apply(ReturnApplyModel model)
        {
            long res = await orderService.ApplyReturnOrderAsync(model.OrderId);
            if(res<=0)
            {
                return new ApiResult { status = 0, msg = "申请退单失败" };
            }
            return new ApiResult { status = 1, msg = "申请退单成功" };
        }
    }    
}