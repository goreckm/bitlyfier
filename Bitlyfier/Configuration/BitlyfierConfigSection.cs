using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Bitlyfier.Configuration
{
    public class BitlyfierConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("apiLogin")]
        public string ApiLogin
        {
            get { return base["apiLogin"].ToString(); }
        }

        [ConfigurationProperty("apiKey")]
        public string ApiKey
        {
            get { return base["apiKey"].ToString(); }
        }

        [ConfigurationProperty("encodeUrls", DefaultValue = true)]
        public bool EncodeUrls
        {
            get { return (bool)base["encodeUrls"]; }
        }

        [ConfigurationProperty("cacheTimeout", DefaultValue = 60)]
        public int CacheTimeout
        {
            get { return (int)base["cacheTimeout"]; }
        }
    }
}
