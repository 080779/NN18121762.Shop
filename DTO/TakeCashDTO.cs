using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class TakeCashDTO:BaseDTO
    {
        public long UserId { get; set; }
        public string Mobile { get; set; }
        public string UserCode { get; set; }
        public string NickName { get; set; }
        public long StateId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Poundage { get; set; }
        public string Description { get; set; }
        public string StateName { get; set; }
        public long PayTypeId { get; set; }
        public string PayTypeName { get; set; }
        public PayCodeDTO PayCode { get; set; }
        public BankAccountDTO BankAccount { get; set; }
        public string AdminMobile { get; set; }
    }
}
