using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.User
{
    public class UserTeamListModel
    {
        public long? TeamLevelId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}