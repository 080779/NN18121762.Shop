using Autofac;
using Autofac.Integration.Mvc;
using IMS.IService;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Web.Jobs
{
    public class AutoConfirmJob : IJob
    {
        private static ILog log = LogManager.GetLogger(typeof(AutoConfirmJob));

        public void Execute(IJobExecutionContext context)
        {
            log.Debug("准备执行自动确认收货和自动取消订单后发放佣金等");
            try
            {
                var container = AutofacDependencyResolver.Current.ApplicationContainer;
                using (container.BeginLifetimeScope())
                {
                    var orderService = container.Resolve<IOrderService>();
                    orderService.AutoConfirm();
                }
                log.Debug("执行自动确认收货和自动取消订单后发放佣金等完成");
            }
            catch (Exception ex)
            {
                log.Error($"执行自动确认收货和自动取消订单后发放佣金等出错，错误信息：{ex}");
            }
        }
    }
}