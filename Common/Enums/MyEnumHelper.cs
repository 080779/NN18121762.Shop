using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Common.Enums
{
    public static class MyEnumHelper
    {
        public static string GetEnumName<T>(this int enumTypeId)
        {
            return Enum.GetName(typeof(T), enumTypeId);
        }

        public static EnumModel[] GetEnumList<T>()
        {
            var arrays = Enum.GetValues(typeof(T));
            List<EnumModel> lists = new List<EnumModel>();
            foreach (var item in arrays)
            {
                lists.Add(new EnumModel { id = (int)item, name = item.ToString() });
            }
            return lists.ToArray();
        }

        public class EnumModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
