using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common
{
    public class PageHelper
    {
        public static PageResult GetPagerHtml(long totalCount,int pageSize,int pageIndex,int maxPageCount=10)
        {
            PageResult result = new PageResult();
            //算出来的页数
            int pageCount = (int)Math.Ceiling(totalCount * 1.0f / pageSize);
            int startPageIndex = Math.Max(1, pageIndex - maxPageCount / 2);//第一个页码
            int endPageIndex = Math.Min(pageCount, startPageIndex + maxPageCount - 1);//最后一个页码
            if(pageIndex<=0)
            {
                pageIndex = 1;
            }
            if(pageIndex>endPageIndex)
            {
                pageIndex = endPageIndex;
            }
            if (pageIndex > 1)
            {
                result.Uppage = pageIndex - 1;
            }
            else
            {
                result.Uppage = pageIndex;
            }
            List<Page> lists = new List<Page>();
            for (int i = startPageIndex; i <= endPageIndex; i++)
            {
                Page page = new Page();
                if (i == pageIndex)
                {
                    page.PageIndex = i;
                    page.Current = "active";
                }
                else
                {
                    page.PageIndex = i;
                    page.Current = "";
                }
                lists.Add(page);
            }
            if (pageIndex < endPageIndex)
            {
                result.Nextpage = pageIndex + 1;
            }
            else
            {
                result.Nextpage = pageIndex;
            }
            result.Endpage = endPageIndex;
            result.Pages = lists;
            return result;
        }
    }
    public class Page
    {
        public long PageIndex { get; set; }
        public string Current { get; set; }
    }
    public class PageResult
    {
        public List<Page> Pages { get; set; }
        public int Uppage { get; set; }
        public int Nextpage { get; set; }
        public int Endpage { get; set; }
    }
}
