using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Bitlyfier.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Bitly(this UrlHelper helper, string longUrl)
        {
            return BitlyfierContext.Current.Shorten(longUrl);
        }
    }
}
