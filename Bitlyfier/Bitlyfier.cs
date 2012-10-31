using Bitlyfier.Configuration;
using Bitlyfier.ResponseMessages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bitlyfier
{
    public class Bitlyfier
    {
        private const string ShortenUrl = "https://api-ssl.bitly.com/v3/shorten";

        private readonly IBitlyfierConfiguration configuration;

        public Bitlyfier() : this(new BitlyfierConfiguration()) { }
        public Bitlyfier(IBitlyfierConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration", "You must provide a configuration.");

            this.configuration = configuration;
        }


        //public HttpMessageHandler HttpMessageHandler { get; set; }
        public IBitlyfierConfiguration Configuration
        {
            get { return this.configuration; }
        }


        public ShortenResponse Shorten(string longUrl)
        {
            return ShortenAsync(longUrl).Result;
        }

        public Task<ShortenResponse> ShortenAsync(string longUrl)
        {
            if (longUrl == null)
                throw new ArgumentNullException("longUrl", "longUrl must be provided.");

            if (configuration.EncodeUrls)
                longUrl = HttpUtility.UrlEncode(longUrl);

            var client = new HttpClient();
            var url = ShortenUrl + "?login=" + configuration.ApiLogin + "&apiKey=" + configuration.ApiKey + "&longUrl=" + longUrl + "&format=json";
            
            var stream = client.GetAsync(url);
            return stream.ContinueWith(s =>
                                    {
                                        dynamic json = JObject.Parse(s.Result.Content.ReadAsStringAsync().Result);

                                        var statusCode = json.status_code;
                                        if (json.status_code != 200)
                                            throw new HttpRequestException("Bitly shorten resulted in status code: " + statusCode);

                                        var response = new ShortenResponse
                                        {
                                            Url = json.data.url,
                                            Hash = json.data.hash,
                                            GlobalHash = json.data.global_hash,
                                            LongUrl = json.data.long_url,
                                            IsNewHash = json.data.new_hash == 1
                                        };

                                        return response;
                                    });
        }
    }
}
