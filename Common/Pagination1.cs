using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Common
{
    public class Pagination1
    {
        /// <summary>
        /// 每页数据条数,默认10条
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总数据条数，默认0
        /// </summary>
        public long TotalCount { get; set; }
        /// <summary>
        /// 当前页码（从 1 开始），默认1
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 最多的页码数，默认10
        /// </summary>
        public int MaxPagerCount { get; set; }
        public List<Page> Pages { get; set; }
        public int PageCount { get; set; }

        public Pagination1()
        {
            this.PageSize = 10;
            this.TotalCount = 0;
            this.PageIndex = 1;
            this.MaxPagerCount = 10;
        }

        public Pagination1(int pageIndex, int pageSize, long totalCount)
        {
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
            this.PageIndex = pageIndex;
            this.MaxPagerCount = 10;
        }

        public string GetPagerHtml()
        {
            StringBuilder sb = new StringBuilder();
            //算出来的页数
            PageCount = (int)Math.Ceiling(TotalCount * 1.0f / PageSize);
            int startPageIndex = Math.Max(1, PageIndex - MaxPagerCount / 2);//第一个页码
            int endPageIndex = Math.Min(PageCount, startPageIndex + MaxPagerCount - 1);//最后一个页码
            if(PageIndex>1)
            {
                sb.Append("<li><a onclick='getPage(").Append(PageIndex-1).AppendLine(")'>上一页</a></li>");
            }
            else
            {
                sb.Append("<li><a onclick='getPage(").Append(PageIndex).AppendLine(")'>上一页</a></li>");
            }
            List<Page> lists = new List<Page>();
            for (int i = startPageIndex; i <= endPageIndex; i++)
            {
                Page page = new Page();
                if (i == PageIndex)
                {
                    page.PageIndex = i;
                    page.Current = "active";
                    sb.Append("<li onclick='getPage(").Append(i).Append(")' class='active'><a>").Append(i).AppendLine("</a></li>");
                }
                else
                {
                    page.PageIndex = i;
                    page.Current = "";
                    sb.Append("<li onclick='getPage(").Append(i).Append(")'><a>").Append(i).AppendLine("</a></li>");
                }
                lists.Add(page);
            }
            if(PageIndex>endPageIndex)
            {
                sb.Append("<li><a onclick='getPage(").Append(PageIndex + 1).AppendLine(")'>下一页</a></li>");
            }
            else
            {
                sb.Append("<li><a onclick='getPage(").Append(PageIndex).AppendLine(")'>下一页</a></li>");
            }
            sb.Append("<li><a onclick='getPage(").Append(endPageIndex).AppendLine(")'>尾页</a></li>");
            Pages = lists;
            return sb.ToString();
        }
        public class Page
        {
            public long PageIndex { get; set; }
            public string Current { get; set; }
        }
    }
}
