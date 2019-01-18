using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.User;
using IMS.Web.WxPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class TestController : Controller
    {
        public ActionResult List()
        {
            WeChatPay w = new WeChatPay();
            var sd= HttpClientHelper.ToKeyValue(w);

            var df = HttpClientHelper.BuildParam(w);
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload(listres imgList)
        {
            return View();
        }
        public ActionResult getres()
        {
            return Json(new AjaxResult { Status = 1 ,Data=new listres() });
        }
        public class file
        {
            public string src { get; set; }
        }
        public class listres
        {
            public List<file> imgList { get; set; }
        }
    }
}