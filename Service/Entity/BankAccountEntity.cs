using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 银行账号实体类
    /// </summary>
    public class BankAccountEntity : BaseEntity
    {
        public long UserId { get; set; }
        public virtual UserEntity User { get; set; }
        public string Name { get; set; }//开户人姓名
        public string BankAccount { get; set; }//银行账号
        public string BankName { get; set; }//开户行
        public string Description { get; set; }
        public bool IsNull { get; set; } = false;
    }
}
