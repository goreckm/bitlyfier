using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitlyfier.ResponseMessages
{
    public class ShortenResponse
    {
        public string Url { get; set; }
        public string Hash { get; set; }
        public string GlobalHash { get; set; }
        public string LongUrl { get; set; }
        public bool IsNewHash { get; set; }
    }
}
