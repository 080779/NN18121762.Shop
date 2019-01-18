using Common.Logging;
using IMS.Common;
using IMS.IService;
using IMS.Web.Models.Notice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class NoticeController : ApiController
    {
        private static ILog log = LogManager.GetLogger(typeof(NoticeController));
        public INoticeService noticeService { get; set; }
        //public IOrderService orderService { get; set; }
        public IOrderListService orderListService { get; set; }
        public IAdminService adminService { get; set; }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult> List()
        {
            NoticeSearchResult result= await noticeService.GetModelListAsync(null,null,null,1,100);
            List<NoticeListApiModel> model;
            model = result.Notices.Where(n=>n.IsEnabled==true).Select(n => new NoticeListApiModel { id = n.Id, content = n.Content, code = n.Code }).ToList();
            //await orderService.AutoConfirmAsync();
            //await orderListService.SetDiscountAmountAsync();
            return new ApiResult { status = 1, data = model };
        }
        [AllowAnonymous]
        public async Task<ApiResult> Detail(NoticeDetailModel model)
        {
            var n= await noticeService.GetModelAsync(model.Id);
            if(n==null)
            {
                return new ApiResult { status = 0, msg = "公告不存在" };
            }
            NoticeListApiModel res = new NoticeListApiModel { id = n.Id, content = n.Content, code = n.Code };
            return new ApiResult { status = 1, data = res };
        }

        public async Task<ApiResult> DelAllData(NoticeDetailModel model)
        {
            log.DebugFormat($"清数据库数据开始");
            await adminService.DelAll();
            log.DebugFormat($"清数据库数据完成");
            return new ApiResult { status = 1, msg = "完成" };
        }
    }
}
