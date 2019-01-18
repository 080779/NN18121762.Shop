using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class UserTeamListApiModel
    {
        public List<member> members { get; set; }
        public long pageCount { get; set; }
        public long totalCount { get; set; }
    }
    public class member
    {
        public long id { get; set; }
        public string mobile { get; set; }
        public string nickName { get; set; }
        public long levelId { get; set; }
        public string levelName { get; set; }
        public string status { get; set; }
        public decimal bonusAmount { get; set; }
        public decimal amount { get; set; }
        public decimal buyAmount { get; set; }
        public string headPic { get; set; }
        public string recommender { get; set; }
    }
}