using IMS.Common;
using IMS.Common.Enums;
using IMS.IService;
using IMS.Web.Models.Notice;
using IMS.Web.Models.Slide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{   
    public class PayTypeController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public ApiResult List()
        {
            return new ApiResult { status = 1, data = MyEnumHelper.GetEnumList<PayTypeEnum>() };
        }
    }
}
