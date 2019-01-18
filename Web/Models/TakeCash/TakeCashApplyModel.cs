using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.TakeCash
{
    public class TakeCashApplyModel
    {
        public int PayTypeId { get; set; } = 0;
        public decimal Amount { get; set; }
    }
}