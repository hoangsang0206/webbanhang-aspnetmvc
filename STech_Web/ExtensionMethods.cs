using STech_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web
{
    public static class ExtensionMethods
    {
        public static bool SearchString(string str1, string str2)
        {
            int count = 0;
            string[] strArr = str2.Split(' ');

            foreach (string s in strArr)
            {
                if (str1.ToLower().Contains(s.ToLower()))
                {
                    count++;
                }
            }

            if (count == strArr.Count())
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<Product> SearchName(this IEnumerable<Product> source, string search)
        {

            return source.Where(t => SearchString(t.ProductName, search));
        }
    }
}