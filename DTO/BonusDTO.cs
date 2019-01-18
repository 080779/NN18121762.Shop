using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class BonusDTO : BaseDTO
    {
        public long UserId { get; set; }
        public string UserMobile { get; set; }
        public int TypeID { get; set; }// 
        public decimal Amount { get; set; }// 
        public decimal Revenue { get; set; }// 
        public decimal sf { get; set; }
        public int IsSettled { get; set; }
        public string Source { get; set; }
        public string SourceEn { get; set; }
        public DateTime? SttleTime { get; set; }
        public long FromUserID { get; set; }
        public int? Bonus001 { get; set; }
        public int Bonus002 { get; set; }
        public string Bonus003 { get; set; }
        public string Bonus004 { get; set; }
        public decimal Bonus005 { get; set; }
        public decimal Bonus006 { get; set; }
    }
}
