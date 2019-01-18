using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using IMS.Web.Models.TakeCash;
using IMS.Web.Models.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{    
    public class ShareController : ApiController
    {        
        public IUserService userService { get; set; }
        public ISettingService settingService { get; set; }
        private string APPID = System.Configuration.ConfigurationManager.AppSettings["APPID"];
        private string APPSECRET = System.Configuration.ConfigurationManager.AppSettings["SECRET"];
        private string doMain = System.Configuration.ConfigurationManager.AppSettings["DoMain"];
        [HttpPost]
        public async Task<ApiResult> Get()
        {
            User user = JwtHelper.JwtDecrypt<User>(ControllerContext);
            string path = "";
            var userDTO = await userService.GetModelAsync(user.Id);
            string parmUrl = doMain;
            if(userDTO==null)
            {
                return new ApiResult { status = 0, msg="会员不存在" };
            }
            if (string.IsNullOrEmpty(userDTO.ShareCode))
            {                
                string getTokenUrl = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", APPID, APPSECRET);
                HttpClient httpClient = new HttpClient();
                string res = await HttpClientHelper.GetResponseByGetAsync(httpClient, getTokenUrl);
                if (res.Contains(@"errcode\"))
                {
                    return new ApiResult { status = 1, data = res };
                }
                GetAccessToken getAccessToken = JsonConvert.DeserializeObject<GetAccessToken>(res);
                Parm parm = new Parm();
                parm.scene = userDTO.Mobile;
                string getCodeUrl = string.Format("https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={0}", getAccessToken.access_token);
                var result = await HttpClientHelper.GetResponseStringByPostJsonAsync(httpClient, parm, getCodeUrl);
                path = ImageHelper.SaveByte(result);
                await userService.UpdateShareCodeAsync(user.Id, path);
            }
            else
            {
                path = userDTO.ShareCode;
            }
            return new ApiResult { status = 1, data = parmUrl + path };
        }
        public class Parm
        {
            public string scene { get; set; }
            public string page { get; set; } = "pages/register/register";
            //public string page { get; set; } = "";
            public int width { get; set; } = 430;
            public bool auto_color { get; set; } = false;
            public object line_color { get; set; } = new LineColor();
            public bool is_hyaline { get; set; } = false;
        }
        public class GetAccessToken
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
        }
        public class LineColor
        {
            public string r { get; set; } = "231";
            public string g { get; set; } = "123";
            public string b { get; set; } = "245";
        }
        public class ImgStream
        {
            public string _buffer { get; set; }
        }
    }    
}