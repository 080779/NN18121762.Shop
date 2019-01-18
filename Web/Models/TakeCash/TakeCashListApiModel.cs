using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.TakeCash
{
    public class TakeCashListApiModel
    {
        public IEnumerable<TakeCash> takeCashes { get; set; }
        public long pageCount { get; set; }
    }
    public class TakeCash
    {
        public decimal? amount { get; set; }
        public string description { get; set; }
        public string stateName { get; set; }
        public string payTypeName { get; set; }
        public string bankAccount { get; set; }
        public DateTime createTime { get; set; }
    }
}