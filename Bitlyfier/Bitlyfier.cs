using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bitlyfier.Configuration;
using Microsoft.Server.Common;

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


        public HttpMessageHandler HttpMessageHandler { get; set; }


        public dynamic Shorten(string longUrl)
        {
            return ShortenAsync(longUrl).Result;
        }

        public Task<dynamic> ShortenAsync(string longUrl)
        {
            if (longUrl == null)
                throw new ArgumentNullException("longUrl", "longUrl must be provided.");

            if (configuration.EncodeUrls)
                longUrl = UrlUtility.UrlEncode(longUrl);

            var client = new HttpClient(HttpMessageHandler);
            var url = ShortenUrl + "?login=" + configuration.ApiLogin + "&apiKey=" + configuration.ApiKey + "&longUrl=" + longUrl + "&format=json";
            
            var stream = client.GetStreamAsync(url);
            return stream.ContinueWith(s =>
                                    {
                                        dynamic json = JsonValue.Load(s.Result).AsDynamic();

                                        var statusCode = json.status_code.ReadAs<int>();
                                        if (statusCode != 200)
                                            throw new HttpRequestException("Bitly shorten resulted in status code: " + statusCode);

                                        return json.data;
                                    });
        }
    }
}
