using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.UserCenter
{
    public class UserCenterDetailApiModel
    {
        public string nickName { get; set; }
        public string headPic { get; set; }
        public string qrCode { get; set; }
        public string bankAccount { get; set; }
    }
}