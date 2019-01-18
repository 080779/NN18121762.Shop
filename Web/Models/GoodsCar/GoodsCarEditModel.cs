using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.GoodsCar
{
    public class GoodsCarEditModel
    {
        public long Id{ get; set; }
        public long? Number { get; set; }
        public bool? IsSelected { get; set; }
    }
}