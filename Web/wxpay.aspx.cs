using Common.Logging;
using IMS.IService;
using IMS.Service.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace IMS.Web
{
    public partial class wxpay : System.Web.UI.Page
    {
        private static ILog log = LogManager.GetLogger(typeof(wxpay));
        private IUserService userService = new UserService();
        protected async Task Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/plain";
            StreamReader reader = new StreamReader(Request.InputStream);
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
                Response.Write(fail.ToString());
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
                            Response.Write(success.ToString());
                        }
                        Response.Write(fail.ToString());
                    }
                    else
                    {
                        Response.Write(success.ToString());
                    }

                    log.DebugFormat("支付后表操作：{0},订单号：{1}", id, orderCode.InnerText);
                }
                catch (Exception ex)
                {
                    log.DebugFormat("支付异常：{0},订单号：{1}", ex.ToString(), orderCode.InnerText);
                }
            }
        }
    }
}