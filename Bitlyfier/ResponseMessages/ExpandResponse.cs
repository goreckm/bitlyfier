using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitlyfier.ResponseMessages
{
    public class ExpandResponse
    {
        public string LongUrl { get; set; }
        public string GlobalHash { get; set; }
        public string UserHash { get; set; }
    }
}
