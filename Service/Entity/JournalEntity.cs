using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    public class JournalEntity : BaseEntity
    {
        public string Remark { get; set; }
        public string RemarkEn { get; set; }
        public decimal? InAmount { get; set; }
        public decimal? OutAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public long GoodsId { get; set; } = 0;
        public string OrderCode { get; set; }
        public long UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public int JournalTypeId { get; set; }
        public int CurrencyType { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
