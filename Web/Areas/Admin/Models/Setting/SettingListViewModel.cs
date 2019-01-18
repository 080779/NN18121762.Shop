using IMS.DTO;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Setting
{
    public class SettingListViewModel
    {
        public SettingParm SysTitle { get; set; }
        public SettingParm Phone1 { get; set; }
        public SettingParm Phone2 { get; set; }
        public SettingParm Logo { get; set; }
        public SettingParm About { get; set; }
        public SettingParm Deduct { get; set; }
        public SettingParm Auto { get; set; }
        public SettingParm UnReturn { get; set; }
    }
}