using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bitlyfier.Tests
{
    [TestFixture]
    public class BitlyServiceIntegrationTests
    {
        [Test]
        public void should_return_data_for_valid_url()
        {
            const string longUrl = "http://betaworks.com";
            const string expectedResult = "beta";

            var response = new Bitlyfier().Shorten(longUrl);
            string actualResult = response.GlobalHash;
            
            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(longUrl, response.LongUrl);
        }
    }
}
