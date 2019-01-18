using IMS.Common;
using IMS.DTO;
using IMS.IService;
using IMS.Web.App_Start.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Web.Areas.Admin.Controllers
{
    public class NoticeController : Controller
    {
        private int pageSize = 10;
        public INoticeService noticeService { get; set; }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        [AdminLog("公告栏管理", "查看公告管理列表")]
        public async Task<ActionResult> List(string keyword, DateTime? startTime, DateTime? endTime, int pageIndex = 1)
        {
            var result = await noticeService.GetModelListAsync(keyword, startTime, endTime, pageIndex, pageSize);
            return Json(new AjaxResult { Status = 1, Data = result });
        }
        [ValidateInput(false)]
        [AdminLog("公告栏管理", "添加公告管理")]
        [Permission("公告栏管理_新增公告")]
        public async Task<ActionResult> Add(string code, string content, DateTime failureTime)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "公告标题不能为空" });
            }
            if (string.IsNullOrEmpty(content))
            {
                return Json(new AjaxResult { Status = 0, Msg = "公告内容不能为空" });
            }            
            long id = await noticeService.AddAsync(code, content, failureTime);
            if (id <= 0)
            {
                return Json(new AjaxResult { Status = 0, Msg = "添加公告失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "添加公告成功" });
        }

        public async Task<ActionResult> GetModel(long id)
        {
            NoticeDTO model = await noticeService.GetModelAsync(id);
            return Json(new AjaxResult { Status = 1, Data = model });
        }

        [ValidateInput(false)]
        [AdminLog("公告栏管理", "添加公告管理")]
        [Permission("公告栏管理_修改公告")]
        public async Task<ActionResult> Edit(long id, string code, string content, DateTime failureTime)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new AjaxResult { Status = 0, Msg = "公告标题不能为空" });
            }
            if (string.IsNullOrEmpty(content))
            {
                return Json(new AjaxResult { Status = 0, Msg = "公告内容不能为空" });
            }
            bool flag = await noticeService.UpdateAsync(id, code, content, failureTime);

            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "修改公告失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "修改公告成功" });
        }
        [AdminLog("公告栏管理", "删除公告管理")]
        [Permission("公告栏管理_删除公告")]
        public async Task<ActionResult> Del(long id)
        {
            bool flag = await noticeService.DeleteAsync(id);
            if (!flag)
            {
                return Json(new AjaxResult { Status = 0, Msg = "删除公告失败" });
            }
            return Json(new AjaxResult { Status = 1, Msg = "删除公告成功" });
        }
    }
}