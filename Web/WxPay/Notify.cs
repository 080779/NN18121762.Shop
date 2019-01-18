using IMS.IService;
using IMS.Service.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace IMS.Web.WxPay
{
    /// <summary>
    /// 回调处理基类
    /// 主要负责接收微信支付后台发送过来的数据，对数据进行签名验证
    /// 子类在此类基础上进行派生并重写自己的回调处理过程
    /// </summary>
    public class Notify
    {
        private static ILog log = LogManager.GetLogger(typeof(wxpay));
        private IUserService userService = new UserService();
        public Controller page { get; set; }
        public Notify(Controller page)
        {
            this.page = page;
        }

        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public async Task<string> GetNotifyData()
        {
            //接收从微信后台POST过来的数据
            StreamReader reader = new StreamReader(page.Request.InputStream);
            string xmlData = reader.ReadToEnd();

            log.DebugFormat($"进入微信支付回调，时间：{DateTime.Now}");
            StringBuilder fail = new StringBuilder();
            fail.AppendLine("<xml>");
            fail.AppendLine("<return_code><![CDATA[FAIL]]></return_code>");
            fail.AppendLine("<return_msg></return_msg>");
            fail.AppendLine("<xml>");
            StringBuilder success = new StringBuilder();
            success.AppendLine("<xml>");
            success.AppendLine("<return_code><![CDATA[SUCCESS]]></return_code>");
            success.AppendLine("<return_msg><![CDATA[OK]]></return_msg>");
            success.AppendLine("<xml>");

            if (!xmlData.Contains("SUCCESS"))
            {
                return fail.ToString();
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);
                XmlNode orderCode = xmlDoc.SelectSingleNode("xml/out_trade_no");
                log.DebugFormat("支付前表操作,订单号：{0}", orderCode.InnerText);
                long id = 0;
                try
                {
                    id = await userService.WeChatPayAsync(orderCode.InnerText);
                    if (id <= 0)
                    {
                        if (id == -4)
                        {
                            log.DebugFormat("支付后表操作：{0},订单号：{1}", id, orderCode.InnerText);
                            return success.ToString();
                        }
                        return fail.ToString();
                    }
                    else
                    {
                        log.DebugFormat("支付后表操作：{0},订单号：{1}", id, orderCode.InnerText);
                        return success.ToString();
                    }

                }
                catch (Exception ex)
                {
                    log.DebugFormat("支付异常：{0},订单号：{1}", ex.ToString(), orderCode.InnerText);
                }
            }
            return "success";
        }

        //派生类需要重写这个方法，进行不同的回调处理
        public virtual void ProcessNotify()
        {

        }
    }
}