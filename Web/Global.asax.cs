using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using IMS.Common.Newtonsoft;
using IMS.IService;
using IMS.Web.App_Start;
using IMS.Web.App_Start.Filter;
using IMS.Web.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            log4net.Config.XmlConfigurator.Configure();

            var builder = new ContainerBuilder();//把当前程序集中的 Controller 都注册,不要忘了.PropertiesAutowired()            
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            Assembly[] assemblies = new Assembly[] { Assembly.Load("IMS.Service") };// 获取所有相关类库的程序集
            builder.RegisterAssemblyTypes(assemblies).
                Where(type => !type.IsAbstract && typeof(IServiceSupport).IsAssignableFrom(type)).AsImplementedInterfaces().PropertiesAutowired();
            //type1.IsAssignableFrom(type2):Assign赋值，type1类型的变量是否可以指向type2类型的对象。也就是type2是否实现type1接口/type2是否继承自type1

            //注册系统级别的 DependencyResolver，这样当 MVC 框架创建 Controller 等对象的时候都是管 Autofac 要对象。            
            var container = builder.Build();

            // Set the dependency resolver for MVC.
            var resolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);

            // Set the dependency resolver for Web API.
            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiResolver;            

            GlobalFilters.Filters.Add(new JsonNetActionFilter());
            GlobalFilters.Filters.Add(new SYSExceptionFilter());
            GlobalFilters.Filters.Add(new SYSAuthorizationFilter());
            StartQuartz();
        }

        private void StartQuartz()
        {
            IScheduler sched = new StdSchedulerFactory().GetScheduler();

            JobDetailImpl jobAutoConfirm = new JobDetailImpl("jobAutoConfirm", typeof(AutoConfirmJob));
            CalendarIntervalScheduleBuilder builder = CalendarIntervalScheduleBuilder.Create();
            builder.WithInterval(10, IntervalUnit.Minute);
            IMutableTrigger triggerAutoConfirm = builder.Build();
            triggerAutoConfirm.Key = new TriggerKey("triggerAutoConfirm");
            sched.ScheduleJob(jobAutoConfirm, triggerAutoConfirm); 

            sched.Start();
        }
    }
}
