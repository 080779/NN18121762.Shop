using IMS.Web.WxPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    [AllowAnonymous]
    public class WxPayController : Controller
    {
        public async Task<string> Return()
        {
            Notify notify = new Notify(this);
            return await notify.GetNotifyData();
        }
    }
}