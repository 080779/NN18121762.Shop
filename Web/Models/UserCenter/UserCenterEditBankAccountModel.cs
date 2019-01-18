using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.UserCenter
{
    public class UserCenterEditBankAccountModel
    {
        public string Name { get; set; }//开户人姓名
        public string BankAccount { get; set; }//银行账号
        public string BankName { get; set; }//开户行
    }
}