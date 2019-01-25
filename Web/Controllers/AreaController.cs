using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.TakeCash;
using IMS.Web.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace IMS.Web.Controllers
{    
    public class AreaController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public object Get()
        {
            return MyJsonHelper.GetFileJson("~/js/areas.json");
        }        
    }    
}