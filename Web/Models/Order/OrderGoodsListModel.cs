﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Order
{
    public class OrderGoodsListModel
    {
        public long OrderId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}