using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
    /// <summary>
    /// 收货地址实体类
    /// </summary>
    public class AddressEntity : BaseEntity
    {
        public long UserId { get; set; }
        public string Name { get; set; }//收货人姓名
        public string Mobile { get; set; }//收货人手机号
        public string Address { get; set; }//收货人地址
        public string Description { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
