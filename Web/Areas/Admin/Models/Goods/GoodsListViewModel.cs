using IMS.DTO;
using IMS.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IMS.Common.Pagination;

namespace IMS.Web.Areas.Admin.Models.Goods
{
    public class GoodsListViewModel
    {
        public GoodsDTO[] Goods { get; set; }
        public long PageCount { get; set; }
        public GoodsTypeDTO[] GoodsTypes { get; set; }
        public GoodsSecondTypeDTO[] GoodsSecondTypes { get; set; }
        public GoodsAreaDTO[] GoodsAreas { get; set; }
    }
}