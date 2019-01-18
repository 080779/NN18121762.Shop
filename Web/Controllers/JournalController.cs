using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.Journal;
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
    public class JournalController : ApiController
    {
        public IJournalService journalService { get; set; }
        public IIdNameService idNameService { get; set; }
        public async Task<ApiResult> List(JournalListModel model)
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var res = await journalService.GetModelListAsync(user.Id, null, null, null, null, model.PageIndex, model.PageSize);
            JournalListApiModel result = new JournalListApiModel();
            //result.totalInAmount = res.TotalInAmount;
            //result.totalOutAmount = res.TotalOutAmount;
            result.pageCount = res.PageCount;
            result.journals = res.List.Select(j => new Journal { createTime = j.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), inAmount = j.InAmount, outAmount = j.OutAmount, remark = j.Remark });
            return new ApiResult { status = 1, data = result };
        }        
    }    
}