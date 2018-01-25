using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace FormBot.Evangelism
{
    public static class ObjectUtils
    {
        public static string IntrospectProperties(this object o)
        {
            var sb = new StringBuilder();
            foreach (var p in o.GetType().GetProperties())
            {
                sb.AppendLine($"{p.Name}: {p.GetValue(o)}");
            }
            return sb.ToString();
        }
    }
}