using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class UserRegisterModel
    {
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string TradePassword { get; set; }
        public string RecommendMobile { get; set; }
        public string Code { get; set; }
        public string AvatarUrl { get; set; }
        public string NickName { get; set; }
    }
}