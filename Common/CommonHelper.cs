using JWT;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IMS.Common
{
    public static class CommonHelper
    {
        public static string GetMD5(this string str)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return GetMD5(bytes);
        }
        public static string GetMD5(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] computeBytes = md5.ComputeHash(bytes);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("X").Length == 1 ? "0" +
                   computeBytes[i].ToString("X") : computeBytes[i].ToString("X");
                }
                return result;
            }
        }
        public static string GetMD5(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] computeBytes = md5.ComputeHash(stream);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("X").Length == 1 ? "0" +
                   computeBytes[i].ToString("X") : computeBytes[i].ToString("X");
                }
                return result;
            }
        }
        public static string GetCaptcha(int length)
        {
            char[] data = { 'a', 'c', 'd', 'e', 'f', 'g', 'k', 'm', 'p', 'r', 's', 't', 'w', 'x', 'y', '3', '4', '5', '7', '8' };
            StringBuilder sbCode = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                char ch = data[rand.Next(data.Length)];
                sbCode.Append(ch);
            }
            return sbCode.ToString();
        }
        public static string GetNumberCaptcha(int length)
        {
            char[] data = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            StringBuilder sbCode = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                char ch = data[rand.Next(data.Length)];
                sbCode.Append(ch);
            }
            return sbCode.ToString();
        }
        #region 获取客户端ip地址
        public static string GetWebClientIp()
        {
            string userIP = "未获取用户IP";
            try
            {
                if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null || System.Web.HttpContext.Current.Request.ServerVariables == null)
                {
                    return "";
                }
                string CustomerIP = "";
                //CDN加速后取到的IP simone 090805
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }
                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!String.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (CustomerIP == null)
                    {
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (string.Compare(CustomerIP, "unknown", true) == 0 || String.IsNullOrEmpty(CustomerIP))
                {
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                }
                return CustomerIP;
            }
            catch { }
            return userIP;
        }
        #endregion

        #region 生成订单号
        public static object _lock = new object();
        public static int count = 1;

        public static string GetRandom1()
        {
            lock (_lock)
            {
                if (count >= 10000)
                {
                    count = 1;
                }
                var number = "P" + DateTime.Now.ToString("yyMMddHHmmss") + count.ToString("0000");
                count++;
                return number;
            }
        }


        public static string GetRandom2()
        {
            lock (_lock)
            {
                return "T" + DateTime.Now.Ticks;

            }
        }

        public static string GetRandom3()
        {
            lock (_lock)
            {
                Random ran = new Random();
                return "dd" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ran.Next(1000, 9999).ToString();
            }
        }
        public static string GetRandom4()
        {
            lock (_lock)
            {
                Random ran = new Random();
                return DateTime.Now.ToString("yyyyMMddHHmmssfff") + ran.Next(100, 999).ToString();
            }
        }
        #endregion

        #region 发送短信
        public static string SendMessage2(string PhoneNum, string content,out string state,out string MsgState)
        {
            string user = System.Configuration.ConfigurationManager.AppSettings["SMS_USER"];
            string pass = System.Configuration.ConfigurationManager.AppSettings["SMS_PASS"];
            string product = System.Configuration.ConfigurationManager.AppSettings["SMS_PRODUCT"];
            string sms_switch = System.Configuration.ConfigurationManager.AppSettings["SMS_SWITCH"];
            //string content = string.Format(System.Configuration.ConfigurationManager.AppSettings["SMS_Template1"], smsg);//发送内容
                                                                                                    // if (sms_switch != "Open") return -1;

            MsgState = string.Empty;

            string sname = user;// new lgk.BLL.tb_globeParam().GetModel(" ParamName='MessageName'").ParamVarchar;
            string spwd = pass;// new lgk.BLL.tb_globeParam().GetModel(" ParamName='MessagePsw'").ParamVarchar;
            string scorpid = "";// this.TxtScorpid.Text.Trim();
            string sprdid = product;

            string PostUrl = "http://cf.lmobile.cn/submitdata/service.asmx/g_Submit";

            string postStrTpl = "sname={0}&spwd={1}&scorpid={2}&sprdid={3}&sdst={4}&smsg={5}";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postData = encoding.GetBytes(string.Format(postStrTpl, sname, spwd, scorpid, sprdid, PhoneNum, content));

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = postData.Length;

            Stream newStream = myRequest.GetRequestStream();
            // Send the data.
            newStream.Write(postData, 0, postData.Length);
            newStream.Flush();
            newStream.Close();

            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            if (myResponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string responseData = reader.ReadToEnd();

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(responseData);
                XmlNode stateXmlNode = xd.ChildNodes[1].ChildNodes[0];
                state = stateXmlNode.InnerText;

                //如果成功则保存到数据库
                if (state == "0")
                {
                    MsgState = "";
                }
                else
                {
                    XmlNode msgIDXmlNode = xd.ChildNodes[1].ChildNodes[2];
                    MsgState = msgIDXmlNode.InnerText;
                }
                //反序列化upfileMmsMsg.Text
                //实现自己的逻辑

                //string sql = string.Format(" INSERT INTO [dbo].[tb_message] ([Userid],[MobileNum],[Mcontent],[Flag])VALUES('{0}','{1}','{2}','{3}')", 0, PhoneNum, content + "," + MsgState, state);
                //DbHelperSQL.GetSingle(sql);
                return state;
            }
            else
            {
                state = "-1";
                return "9999";
                //访问失败
            }
        }
        public static string SendMessage(string PhoneNum, string content, out string state, out string msgState)
        {
            state = "111";
            msgState = "123erqewqw";
            return "9999";
        }

        //<!--短信设置-->
        //<add key="SMS_USER" value="dlgxnn07"/>
        //<add key="SMS_PASS" value="d0JAEWy4"/>
        //<!--短信产品号-->
        //<add key="SMS_PRODUCT" value="1012818"/>
        //<!--短信emailTemplateId-->
        //<add key="SMS_Template1" value="您的验证码为{0}，验证码5分钟内有效，请勿转发他人，谢谢合作！【联州商汇】"/>
        //<!--短信emailTemplateId-->
        //<add key="SMS_Template2" value="温馨提示，您于{0}年{1}月{2}日提交的订单{3}已匹配成功，请及时进行操作。【中国红】"/>
        #endregion
    }
}
