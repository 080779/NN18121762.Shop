using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 用户Token实体类
    /// </summary>
    public class UserTokenEntity : BaseEntity
    {
        public long UserId { get; set; }
        public string Token { get; set; }
    }
}
