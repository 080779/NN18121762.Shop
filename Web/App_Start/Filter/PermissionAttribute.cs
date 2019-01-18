using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.App_Start.Filter
{
    /// <summary>
    ///  添加此特性的功能是：需要用户登录才能够浏览网页，如果不需要用户登录，则可以使用AllowAnonymousAttribute属性
    /// 使用：给ASP.NET MVC中的控制器类名或者Action方法上面打上[ValidAuthorizer]标签
    /// 如果不需要验证用户是否登录就可以浏览，则给ASP.NET MVC中的控制器类名或者Action方法上面打上[AllowAnonymous]标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class PermissionAttribute : FilterAttribute
    {
        public string Permission { get; set; }
        public PermissionAttribute(string permission)
        {
            this.Permission = permission;
        }
    }
}