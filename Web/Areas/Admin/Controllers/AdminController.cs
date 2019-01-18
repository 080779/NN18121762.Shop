using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Areas.Admin.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public IAdminService adminService { get; set; }
        public IPermissionService permissionService { get; set; }
        public IPermissionTypeService permissionTypeService { get; set; }
        private int pageSize = 10;
        //[Permission("管理员管理_管理员管理")]
        public ActionResult List()
        {            
            return View();
        }
        [HttpPost]
        //[Permission("管理员管理_管理员管理")]
        [AdminLog("管理员管理", "查看管理员列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            AdminSearchResult result= await adminService.GetModelListAsync("admin", keyword, startTime, endTime, pageIndex, pageSize);          
            ListViewModel model = new ListViewModel();
            model.Admins = result.Admins;
            PermissionTypeDTO[] types = await permissionTypeService.GetModelList();
            List<PermissionType> permissionTypes = new List<PermissionType>();
            foreach (var type in types)
            {
                PermissionType permissionType = new PermissionType();
                permissionType.Name = type.Name;
                PermissionDTO[] permissions = await permissionService.GetByTypeIdAsync(type.Id);
                permissionType.Permissions = permissions.ToList();
                permissionTypes.Add(permissionType);
            }
            model.PermissionTypes = permissionTypes;
            model.PageCount = result.PageCount;
            return Json(new AjaxResult { Status = 1, Data = model });
        }
        //[Permission("管理员管理_管理员管理")]
        [Permission("管理员管理_新增管理员")]
        [AdminLog("管理员管理", "新增管理")]
        public async Task<ActionResult> Add(string mobile,string password)
        {
            string adminMobile = (await adminService.GetModelAsync(Convert.ToInt64(Session["Platform_AdminUserId"]))).Mobile;
            if (string.IsNullOrEmpty(mobile))
            {
                return Json(new AjaxResult { Status = 0, Msg="管理员账号不能为空"});
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "管理员密码不能为空" });
            }
            long res= await adminService.AddAsync(adminMobile,mobile, null, password);
            if(res<=0)
            {
                if(res==-2)
                {
                    return Json(new AjaxResult { Status = 0, Msg = "管理员账户已经存在" });
                }
                return Json(new AjaxResult { Status = 0, Msg = "添加管理员失败" });
            }
            return Json(new AjaxResult { Status = 1,Msg= "添加管理员成功", Data = "/admin/admin/list" });
        }
        //[Permission("管理员管理_管理员管理")]
        [Permission("管理员管理_修改权限")]
        [AdminLog("管理员管理", "修改权限")]
        public async Task<ActionResult> EditPermission(long id, List<long> permissionIds)
        {
            if (permissionIds==null)
            {
                permissionIds = new List<long>();
            }
            bool res = await adminService.UpdateAsync(id,permissionIds.ToArray());
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "编辑管理员权限失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑管理员权限成功", Data = "/admin/admin/list" });
        }
        //[Permission("管理员管理_管理员管理")]
        [Permission("管理员管理_修改密码")]
        [AdminLog("管理员管理", "修改密码")]
        public async Task<ActionResult> EditPassword(long id, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return Json(new AjaxResult { Status = 0, Msg = "请选择权限项" });
            }
            bool res = await adminService.UpdateAsync(id, password);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "编辑管理员密码失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "编辑管理员密码成功", Data = "/admin/admin/list" });
        }
        public async Task<ActionResult> GetPerm(long id,List<long> permissionIds)
        {
            if(permissionIds==null)
            {
                permissionIds = new List<long>();
            }
            PermissionTypeDTO[] types = await permissionTypeService.GetModelList();
            List<PermissionType> permissionTypes = new List<PermissionType>();
            foreach (var type in types)
            {
                PermissionType permissionType = new PermissionType();
                permissionType.Name = type.Name;
                PermissionDTO[] permissions = await permissionService.GetByTypeIdAsync(type.Id);
                foreach(var perm in permissions)
                {
                    if(permissionIds.Contains(perm.Id))
                    {
                        perm.IsChecked = true;
                    }
                }
                permissionType.Permissions = permissions.ToList();
                permissionTypes.Add(permissionType);
            }
            return Json(new AjaxResult { Status = 1, Data = permissionTypes });
        }
        //[Permission("管理员管理_管理员管理")]
        [Permission("管理员管理_冻结管理")]
        [AdminLog("管理员管理", "冻结账户")]
        public async Task<ActionResult> Frozen(long id)
        {
            bool res= await adminService.FrozenAsync(id);
            if(!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "冻结、解冻管理员账号操作失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "冻结、解冻管理员账号操作成功" });
        }
        //[Permission("管理员管理_管理员管理")]
        [Permission("管理员管理_删除用户")]
        [AdminLog("管理员管理", "删除账户")]
        public async Task<ActionResult> Del(long id)
        {
            bool res = await adminService.DeleteAsync(id);
            if (!res)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除管理员账户失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除管理员账户成功" });
        }
    }
}