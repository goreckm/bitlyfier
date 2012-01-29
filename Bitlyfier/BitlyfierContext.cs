using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Bitlyfier
{
    public class BitlyfierContext
    {
        private const string CacheKeyPrefixShorten = "Bitlyfier_Shorten_";

        private static readonly Lazy<BitlyfierContext> lazy = 
            new Lazy<BitlyfierContext>(() => new BitlyfierContext());

        public static BitlyfierContext Current { get { return lazy.Value; } }

        private BitlyfierContext() { }


        protected Cache Cache { get { return HttpContext.Current.Cache; } }

        public string Shorten(string longUrl)
        {
            var result = Cache.Get(CacheKeyPrefixShorten + longUrl) as string;
            if (result == null)
            {
                var bitlyfier = new Bitlyfier();
                result = bitlyfier.ShortenAsync(longUrl).Result.Url;
                Cache.Insert(CacheKeyPrefixShorten + longUrl, result, null, DateTime.UtcNow.AddMinutes(bitlyfier.Configuration.CacheTimeout), Cache.NoSlidingExpiration);
            }
            
            return result;
        }
    }
}
