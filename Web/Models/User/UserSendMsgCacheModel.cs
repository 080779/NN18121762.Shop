using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class UserSendMsgCacheModel
    {
        public string Code { get; set; }
        public long ErrorTime { get; set; }
        public DateTime CanSendTime { get; set; }
    }
}