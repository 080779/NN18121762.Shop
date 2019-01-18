using IMS.Web.App_Start.Filter;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace IMS.Web.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        { 
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.EnsureInitialized();

            config.Filters.Add(new ApiSYSAuthorizationFilter());
            //ReturnJsonSerializerSettings(); //json返回格式化设置
        }

        /// <summary>
        /// json返回格式化设置
        /// </summary>
        private static void ReturnJsonSerializerSettings()
        {
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环引用，如果设置为Error，则遇到循环引用的时候报错（建议设置为Error，这样更规范）
            json.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";//日期格式化
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//json中属性开头字母小写的驼峰命名
            json.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
        }
    }
}