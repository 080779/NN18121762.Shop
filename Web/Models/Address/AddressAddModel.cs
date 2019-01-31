using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Address
{
    public class AddressAddModel
    {
        public string Name { get; set; }//收货人姓名
        public string Mobile { get; set; }//收货人手机号
        public string Sheng { get; set; }
        public string Shi { get; set; }
        public string Qu { get; set; }
        public string Address { get; set; }//收货人地址
        public bool IsDefault { get; set; }
    }
}