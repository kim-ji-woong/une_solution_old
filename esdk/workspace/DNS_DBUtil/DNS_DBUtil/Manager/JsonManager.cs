using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace dnsDBUtil
{
    public static class JsonManager
    {
        public static string Serialize(object items)
        {
            string ret = JsonConvert.SerializeObject(items);

            //string ret = "";
            //string[] keys = items.Keys.ToArray();
            //string[] values = items.Values.ToArray();
            //
            //if (items.Count > 1)
            //{
            //    for (int i = 0; i < items.Count; i++)
            //    {
            //        if (i == 0)
            //        {
            //            ret += string.Format("{{\"{0}\" : \"{1}\",", keys[i], values[i]);
            //        }
            //        else if (i == items.Count - 1)
            //        {
            //            ret += string.Format("\"{0}\" : \"{1}\"}}", keys[i], values[i]);
            //        }
            //        else
            //        {
            //            ret += string.Format("\"{0}\" : \"{1}\",", keys[i], values[i]);
            //        }
            //    }
            //}
            //else if (items.Count == 1)
            //{
            //    ret = string.Format("{{\"{0}\" : \"{1}\"}}", keys[0], values[0]);
            //}
            //else
            //{
            //    // Not Defined
            //}

            return ret;
        }

        public static T Deserialize<T>(string value)
        {
            T ret = JsonConvert.DeserializeObject<T>(value);

            //string[] ret = null;
            //string[] seperatingStr = { "\"," };
            //
            //if (value.Contains("\\"))
            //{
            //    value = value.Replace("\\", "");
            //}
            //
            //if (value.Contains(","))
            //{
            //    value = value.Replace("[", "");
            //    value = value.Replace("]", "");
            //    ret = value.Split(seperatingStr, StringSplitOptions.RemoveEmptyEntries);
            //
            //    for (int i = 0; i < ret.Length; i++)
            //    {
            //        if (ret[i].StartsWith("\""))
            //        {
            //            ret[i] = ret[i].Remove(0, 1);
            //        }
            //
            //        if (ret[i].EndsWith("\""))
            //        {
            //            ret[i] = ret[i].Remove(ret[i].Length - 1, 1);
            //        }
            //    }
            //
            //    //value = value.Replace("\"", "");
            //}
            //else
            //{
            //    // Not Defined
            //}

            return (T)Convert.ChangeType(ret, typeof(T));
        }
    }
}
