using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Venice.Infrastructure
{
    public static class Utils
    {
        public static string ToCommaDelimitedList<T>(this IEnumerable<T> t)
        {
            return string.Join(",", t.Select(o => o.ToString()).ToArray());
        }

        public static string ToDbCleanString(this string t)
        {
            return t.Replace("'", "''");
        }
    }
}