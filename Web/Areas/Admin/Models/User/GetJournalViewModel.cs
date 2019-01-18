using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.User
{
    public class GetJournalViewModel
    {
        public JournalDTO[] Journals { get; set; }
        public int PageCount { get; set; }
        public List<Page> Pages { get; set; }
    }
}