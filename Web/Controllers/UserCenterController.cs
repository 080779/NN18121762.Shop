using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.UserCenter;
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

namespace IMS.Web.Controllers
{
    public class UserCenterController : ApiController
    {
        private string doMain = System.Configuration.ConfigurationManager.AppSettings["DoMain"];
        public IUserService userService { get; set; }
        public IBankAccountService bankAccountService { get; set; }
        public IPayCodeService payCodeService { get; set; }
        public ISettingService settingService { get; set; }

        [HttpPost]
        public async Task<ApiResult> Info()
        {
            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            UserDTO result = await userService.GetModelAsync(user.Id);
            UserCenterInfoApiModel model = new UserCenterInfoApiModel();
            model.amount = result.Amount;
            model.frozenAmount = result.FrozenAmount;
            model.bonusAmount = result.BonusAmount+result.FrozenAmount;
            model.buyAmount = result.BuyAmount + (await userService.GetTeamBuyAmountAsync(user.Id));
            model.createTime = result.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            if(!string.IsNullOrEmpty(result.HeadPic))
            {
                if(result.HeadPic.Contains("https:"))
                {
                    model.headPic = result.HeadPic;
                }
                else
                {
                    model.headPic = parm + result.HeadPic;
                }                
            }
            else
            {
                model.headPic = parm;
            }           
            model.id = result.Id;
            model.levelId = result.LevelId;
            model.levelName = result.LevelName;
            model.mobile = result.Mobile;
            model.nickName = result.NickName;
            model.recommonder = result.RecommendCode;
            model.userCode = result.UserCode;
            return new ApiResult { status = 1, data = model };
        }

        [HttpPost]
        public async Task<ApiResult> Detail()
        {
            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            UserDTO userdto = await userService.GetModelAsync(user.Id);
            PayCodeDTO[] payCodes = await payCodeService.GetModelByUserIdAsync(user.Id);
            BankAccountDTO bankAccount = await bankAccountService.GetModelByUserIdAsync(user.Id);
            UserCenterDetailApiModel model = new UserCenterDetailApiModel();
            model.qrCode = payCodes.Count() <= 0 ? "" : parm + payCodes.First().CodeUrl;
            model.bankAccount = bankAccount == null ? "" : bankAccount.BankAccount;
            if (userdto != null)
            {
                if (!string.IsNullOrEmpty(userdto.HeadPic))
                {
                    if (userdto.HeadPic.Contains("https:"))
                    {
                        model.headPic = userdto.HeadPic;
                    }
                    else
                    {
                        model.headPic = parm + userdto.HeadPic;
                    }
                }
                else
                {
                    model.headPic = parm;
                }
                model.nickName = userdto.NickName;
            }
            return new ApiResult { status = 1, data = model };
        }
        [HttpPost]
        public async Task<ApiResult> EditHeadPic()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象
            HttpPostedFileBase _upfile = context.Request.Files["File"];
            if (_upfile == null)
            {
                return new ApiResult { status = 0, msg = "图片文件不能为空" };
            }
            string res = ImageHelper.SaveImage(_upfile);

            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            bool flag = await userService.UpdateInfoAsync(user.Id, null, res);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = HttpUtility.UrlEncode("头像添加修改失败", Encoding.UTF8) };
            }
            return new ApiResult { status = 1, data = parm + res };
        }
        [HttpPost]
        public async Task<ApiResult> EditQrCode()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象
            HttpPostedFileBase _upfile = context.Request.Files["File"];
            if (_upfile == null)
            {
                return new ApiResult { status = 0, msg = "图片文件不能为空" };
            }
            string res = ImageHelper.SaveImage(_upfile);

            string parm = doMain;
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            PayCodeDTO[] payCodes = await payCodeService.GetModelByUserIdAsync(user.Id);
            if(payCodes.Count()<=0)
            {
                long id= await payCodeService.AddAsync(user.Id, "微信收款码", res, null);
                if(id<=0)
                {
                    return new ApiResult { status = 0, msg = HttpUtility.UrlEncode("收款码添加修改失败", Encoding.UTF8) };
                }
            }
            else
            {
                bool flag = await payCodeService.UpdateAsync(payCodes.First().Id,null,res,null);
                if (!flag)
                {
                    return new ApiResult { status = 0, msg = HttpUtility.UrlEncode("收款码添加修改失败", Encoding.UTF8) };
                }
            }
            return new ApiResult { status = 1, data = parm + res };
        }
        [HttpPost]
        public async Task<ApiResult> EditNickName(UserCenterEditNickNameModel model)
        {
            if (string.IsNullOrEmpty(model.NickName))
            {
                return new ApiResult { status = 0, msg = "昵称不能为空" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            bool flag = await userService.UpdateInfoAsync(user.Id, model.NickName, null);
            if (!flag)
            {
                return new ApiResult { status = 0, msg = "昵称添加修改失败" };
            }
            return new ApiResult { status = 1, msg = "昵称添加修改成功" };
        }

        [HttpPost]
        public async Task<ApiResult> ResetPwd(UserCenterResetPwdModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                return new ApiResult { status = 0, msg = "原登录密码不能为空" };
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                return new ApiResult { status = 0, msg = "新登录密码不能为空" };
            }
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            long id = await userService.ResetPasswordAsync(user.Id, model.Password, model.NewPassword);
            if (id == -1)
            {
                return new ApiResult { status = 0, msg = "用户不存在" };
            }
            if (id == -2)
            {
                return new ApiResult { status = 0, msg = "原登录密码错误" };
            }
            return new ApiResult { status = 1, msg = "密码修改成功！" };
        }
    }
}