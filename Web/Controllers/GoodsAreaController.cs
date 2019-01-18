using IMS.Common;
using IMS.IService;
using IMS.Web.Models.GoodsArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace IMS.Web.Controllers
{
    public class GoodsAreaController : ApiController
    {
        public IGoodsAreaService goodsAreaService { get; set; }
        [HttpPost]
        public async Task<ApiResult> List()
        {
            GoodsAreaSearchResult result = await goodsAreaService.GetModelListAsync(null, null, null, 1, 100);
            List<GoodsAreaListApiModel> model;
            model = result.GoodsAreas.Select(g => new GoodsAreaListApiModel { id = g.Id, title = g.Title, description = g.Description }).ToList();
            return new ApiResult { status = 1, data = model };
        }
    }
}
