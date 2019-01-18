using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web.App_Start.Filter
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ExportExcelNameAttribute : Attribute
    {
        public string Name { get; set; }
        public ExportExcelNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}