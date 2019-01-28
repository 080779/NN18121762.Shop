using IMS.Common;
using IMS.Common.Enums;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private int pageSize = 10;
        public IUserService userService { get; set; }
        public IIdNameService idNameService { get; set; }
        public ISettingService settingService { get; set; }
        //public IOrderService orderService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("会员管理", "查看用户管理列表")]
        public async Task<ActionResult> List(int? levelId,string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            //await orderService.AutoConfirmAsync();
            //long levelId = await idNameService.GetIdByNameAsync("会员等级");
            var result = await userService.GetModelListAsync(levelId, keyword, startTime, endTime, pageIndex, pageSize);
            var set1 = new SettingDTO();
            UserListViewModel model = new UserListViewModel();
            model.ThreePlay = new SettingModel { Id = set1.Id, Name = set1.Name, Parm = set1.Param };
            model.PageCount = result.PageCount; 
            model.Users = result.Users;
            model.Levels = MyEnumHelper.GetEnumList<LevelEnum>();
            //model.UserUps = (await settingService.GetModelListAsync("会员升级")).Select(s => new SettingModel { Id = s.Id, Parm = s.Param, Name=s.Name}).ToList();
            //model.Discounts = (await settingService.GetModelListAsync("会员优惠")).Select(s => new SettingModel { Id = s.Id, Parm = s.Param, Name=s.Name}).ToList();
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        [Permission("会员管理_新增会员")]
        [AdminLog("会员管理", "添加用户")]
        public async Task<ActionResult> Add(string mobile,string recommendMobile,string password)
        {
            if(string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "会员账号不能为空" });
            }
            if (!Regex.IsMatch(mobile, @"^1\d{10}$"))
            {
                return Json(new AjaxResult { Status = 0, Msg = "注册手机号格式不正确" });
            }
            if (string.IsNullOrEmpty(recommendMobile))
            {
                return Json(new AjaxResult { Status = 0, Msg = "推荐人账号不能为空" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            int levelId= 1;
            long id= await userService.AddAsync(mobile,password, "" , levelId,recommendMobile,null,null);
            if(id<=0)
            {
                if (id == -1)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "会员账号已经存在" });
                }
                if (id == -2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "推荐人不存在" });
                } 
                return Json(new AjaxResult { Status = 0, Msg = "会员添加失败" });
            }            
            return Json(new AjaxResult { Status = 1, Msg = "会员添加成功" });
        }
        
        [HttpPost]
        [AdminLog("会员管理", "账户充值")]
        [Permission("会员管理_账户充值")]
        public async Task<ActionResult> Charge(long id,int type,string amount)
        {
            decimal Amount;
            if(!decimal.TryParse(amount, out Amount))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请输入数字" });
            }
            if(Amount < 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "金额不能小于零" });
            }
            var res = await userService.ChargeAsync(id,type,Amount);
            if(res<=0)
            {
                if (res == -2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "余额不足" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "充值失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "充值成功" });
        }
        [AdminLog("会员管理", "重置用户密码")]
        [Permission("会员管理_重置密码")]
        public async Task<ActionResult> ResetPwd(long id, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "登录密码不能为空" });
            }
            long res = await userService.ResetPasswordAsync(id,password);
            if (res <= 0)
            {
                if (id == -1)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "会员不存在" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "重置密码失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "重置密码成功" });
        }
        [AdminLog("会员管理", "冻结用户")]
        [Permission("会员管理_冻结用户")]
        public async Task<ActionResult> Frozen(long id)
        {
            bool res= await userService.FrozenAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻用户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻用户成功" });
        }
        [AdminLog("会员管理", "删除用户")]
        [Permission("会员管理_删除用户")]
        public async Task<ActionResult> Delete(long id)
        {
            bool res = await userService.DeleteAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除用户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除用户成功" });
        }
    }
}