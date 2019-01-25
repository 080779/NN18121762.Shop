using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Common
{
    public static class MyJsonHelper
    {
        public static object GetFileJson(string path)
        {
            string json = string.Empty;
            string filepath= HttpContext.Current.Server.MapPath(path);
            using (FileStream fs = new FileStream(filepath, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            return JsonConvert.DeserializeObject(json);
        }
    }
}
