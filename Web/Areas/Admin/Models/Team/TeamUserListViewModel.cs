using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Enums.MyEnumHelper;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Team
{
    public class TeamUserListViewModel
    {
        public UserDTO[] Users { get; set; }
        public EnumModel[] Levels { get; set; }
        public long PageCount { get; set; }
        public EnumModel[] TeamLevels { get; set; }
    }
}