using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class JournalDTO:BaseDTO
    {
        public string Remark { get; set; }
        public string RemarkEn { get; set; }
        public decimal? InAmount { get; set; }
        public decimal? OutAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public int JournalTypeId { get; set; }
        public string JournalTypeName { get; set; }
        public long UserId { get; set; }
        public string Mobile { get; set; }
        public string NickName { get; set; }
        public long GoodsId { get; set; }
        public string OrderCode { get; set; }
        public bool IsEnabled { get; set; }
        public int CurrencyType { get; set; }
    }
}
