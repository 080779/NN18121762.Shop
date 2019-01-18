using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
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
    public class UserTeamController : ApiController
    {
        private string doMain = System.Configuration.ConfigurationManager.AppSettings["DoMain"];
        public IUserService userService { get; set; }
        public ISettingService settingService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List(UserTeamListModel model)
        {
            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            var res= await userService.GetModelTeamListAsync(user.Id,model.TeamLevelId,null,null,null,model.PageIndex, model.PageSize);
            UserTeamListApiModel result = new UserTeamListApiModel();
            result.members = res.Members.Select(u => new member
            {
                id = u.Id,
                mobile = u.Mobile,
                nickName = u.NickName,
                levelId = u.LevelId,
                levelName = u.LevelName,
                status = (u.IsEnabled == true ? "已启用" : "已冻结"),
                bonusAmount = u.BonusAmount,
                amount = u.Amount,
                buyAmount = u.BuyAmount + (userService.GetTeamBuyAmount(u.Id)),
                recommender = u.RecommendCode,
                headPic = (!string.IsNullOrEmpty(u.HeadPic) && u.HeadPic.Contains("https://")) ? u.HeadPic : parm + u.HeadPic
            }).ToList();
            result.totalCount = res.TotalCount;
            result.pageCount = res.PageCount;
            return new ApiResult { status = 1,data= result };
        }
        [HttpPost]
        [AllowAnonymous]
        public ApiResult Levels()
        {
            return new ApiResult { status = 1, data = MyEnumHelper.GetEnumList<TeamLevelEnum>() };
        }
    }    
}