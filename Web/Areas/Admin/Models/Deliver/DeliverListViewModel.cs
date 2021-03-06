﻿using IMS.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Enums.MyEnumHelper;

namespace IMS.Web.Areas.Admin.Models.Deliver
{
    public class DeliverListViewModel
    {
        public OrderDTO[] Orders { get; set; }
        public long PageCount { get; set; }
        public EnumModel[] OrderStates { get; set; }
    }
}