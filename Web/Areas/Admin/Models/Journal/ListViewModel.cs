using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Journal
{
    public class ListViewModel
    {
        public JournalDTO[] List { get; set; }
        public long PageCount { get; set; }
    }
}