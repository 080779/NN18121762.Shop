using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Address
{
    public class AddressListModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}