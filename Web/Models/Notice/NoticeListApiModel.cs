using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Notice
{
    public class NoticeListApiModel
    {
        public long id { get; set; }
        public string code { get; set; }
        public string content { get; set; }
    }
}