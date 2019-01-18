using IMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.WxPay
{
    public class WeChatPay
    {
        public string appid { get; set; }= System.Configuration.ConfigurationManager.AppSettings["APPID"];
        public string mch_id { get; set; } = System.Configuration.ConfigurationManager.AppSettings["MCH_ID"];
        public string nonce_str { get; set; } = CommonHelper.GetCaptcha(10);
        public string sign_type { get; set; } = "MD5";
        public string body { get; set; } = "test";
        public string detail { get; set; } = "testdetail";
        public string out_trade_no { get; set; } = CommonHelper.GetCaptcha(12);
        public string fee_type { get; set; } = "CNY";
        public string total_fee { get; set; } = "1";
        public string notify_url { get; set; } = System.Configuration.ConfigurationManager.AppSettings["DoMain"] + "/wxpay/return";
        public string trade_type { get; set; } = "JSAPI";
        public string openid { get; set; } = "";
    }
}