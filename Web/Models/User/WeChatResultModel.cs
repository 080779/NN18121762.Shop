using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class WeChatResultModel
    {
        public string OpenId { get; set; }
        public string Session_key { get; set; }
        public string UnionId { get; set; }
    }
}