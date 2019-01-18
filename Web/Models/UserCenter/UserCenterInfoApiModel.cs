using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.UserCenter
{
    public class UserCenterInfoApiModel
    {
        public long id { get; set; }
        public string userCode { get; set; }
        public string mobile { get; set; }
        public string nickName { get; set; }
        public string headPic { get; set; }
        public decimal amount { get; set; }
        public decimal frozenAmount { get; set; }
        public decimal buyAmount { get; set; }
        public decimal bonusAmount { get; set; }
        public long levelId { get; set; }
        public string levelName { get; set; }
        public string recommonder { get; set; }
        public string createTime { get; set; }
    }
}