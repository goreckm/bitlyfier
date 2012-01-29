using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bitlyfier.Configuration;
using Microsoft.Server.Common;
using Bitlyfier.ResponseMessages;

namespace Bitlyfier
{
    public class Bitlyfier
    {
        private const string ShortenUrl = "https://api-ssl.bitly.com/v3/shorten";
        private const string ExpandUrl = "https://api-ssl.bitly.com/v3/expand";

        private readonly IBitlyfierConfiguration configuration;

        public Bitlyfier() : this(new BitlyfierConfiguration()) { }
        public Bitlyfier(IBitlyfierConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration", "You must provide a configuration.");

            this.configuration = configuration;
        }


        public HttpMessageHandler HttpMessageHandler { get; set; }
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

                                        var response = new ShortenResponse
                                        {
                                            Url = json.data.url.ReadAs<string>(),
                                            Hash = json.data.hash.ReadAs<string>(),
                                            GlobalHash = json.data.global_hash.ReadAs<string>(),
                                            LongUrl = json.data.long_url.ReadAs<string>(),
                                            IsNewHash = json.data.new_hash.ReadAs<int>() == 1
                                        };

                                        return response;
                                    });
        }

        public Task<IEnumerable<ExpandResponse>> ExpandAsync(string shortUrl = null, string hash = null)
        {
            IList<string> shortUrlList = null;
            IList<string> hashList = null;

            if (shortUrl != null)
                shortUrlList = new List<string> { shortUrl };

            if (hash != null)
                hashList = new List<string> { hash };

            return ExpandAsync(shortUrlList, hashList);
        }

        public Task<IEnumerable<ExpandResponse>> ExpandAsync(IList<string> shortUrl = null, IList<string> hash = null)
        {
            if (shortUrl == null && hash == null)
                throw new ArgumentException("One of shortUrl or hash arguments must not be null.");

            var sb = new StringBuilder(ExpandUrl);
            sb.Append("?login=" + configuration.ApiLogin + "&apiKey=" + configuration.ApiKey);

            if (shortUrl != null)
            {
                foreach (var shortUrlItem in shortUrl)
                {
                    sb.Append("&shortUrl=");
                    sb.Append(configuration.EncodeUrls ? UrlUtility.UrlEncode(shortUrlItem) : shortUrlItem);
                }
            }

            if (hash != null)
            {
                foreach (var hashItem in hash)
                {
                    sb.Append("&hash=");
                    sb.Append(hashItem);
                }
            }

            sb.Append("&format=json");
            var url = sb.ToString();

            var client = new HttpClient(HttpMessageHandler);
            var stream = client.GetStreamAsync(url);
            return stream.ContinueWith(s =>
            {
                dynamic json = JsonValue.Load(s.Result).AsDynamic();

                var statusCode = json.status_code.ReadAs<int>();
                if (statusCode != 200)
                    throw new HttpRequestException("Bitly expand resulted in status code: " + statusCode);

                var response = new List<ExpandResponse>();
                foreach (var entry in json.data.expand)
                {
                    response.Add(new ExpandResponse
                    {
                        LongUrl = entry.long_url.ReadAs<string>(),
                        GlobalHash = entry.global_hash.ReadAs<string>(),
                        UserHash = entry.user_hash.ReadAs<string>()
                    });
                }
                    
                return response.AsEnumerable();
            });
        }
    }
}
