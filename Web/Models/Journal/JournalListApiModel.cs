using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Journal
{
    public class JournalListApiModel
    {
        public IEnumerable<Journal> journals { get; set; }
        public decimal? totalInAmount { get; set; }
        public decimal? totalOutAmount { get; set; }
        public long pageCount { get; set; }
    }
    public class Journal
    {
        public string createTime { get; set; }
        public string remark { get; set; }
        public decimal? inAmount { get; set; }
        public decimal? outAmount { get; set; }
    }
}