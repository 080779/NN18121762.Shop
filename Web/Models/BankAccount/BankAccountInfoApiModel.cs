using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.BankAccount
{
    public class BankAccountInfoApiModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public string bankAccount { get; set; }
        public string bankName { get; set; }
    }
}