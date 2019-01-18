using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Goods
{
    public class GoodsListModel
    {
        public bool? IsHotSale { get; set; }
        public long? TypeId { get; set; }
        public long? SecondTypeId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}