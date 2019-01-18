using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class NavBarDTO:BaseDTO
    {
        public long MenuId { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public long ParentId { get; set; }
        /// <summary>
        ///排序
        /// </summary>
        public long Sort { get; set; }
        public bool IsLock { get; set; }
    }
}
