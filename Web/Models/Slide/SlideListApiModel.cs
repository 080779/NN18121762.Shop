using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.Models.Slide
{
    public class SlideListApiModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string imgUrl { get; set; }
    }
}