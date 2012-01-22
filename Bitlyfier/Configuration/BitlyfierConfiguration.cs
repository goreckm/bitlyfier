using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bitlyfier.Configuration
{
    public class BitlyfierConfiguration : IBitlyfierConfiguration
    {
        private readonly BitlyfierConfigSection configSection =
            ConfigurationManager.GetSection("Bitlyfier") as BitlyfierConfigSection;

        public BitlyfierConfiguration()
        {
            ApiLogin = configSection == null ? "bitlyapidemo" : configSection.ApiLogin;
            ApiKey = configSection == null ? "R_0da49e0a9118ff35f52f629d2d71bf07" : configSection.ApiKey;
            EncodeUrls = configSection == null || configSection.EncodeUrls;
        }

        public string ApiLogin { get; set; }
        public string ApiKey { get; set; }
        public bool EncodeUrls { get; set; }
    }


    public interface IBitlyfierConfiguration
    {
        string ApiLogin { get; set; }
        string ApiKey { get; set; }

        bool EncodeUrls { get; set; }
    }
}
