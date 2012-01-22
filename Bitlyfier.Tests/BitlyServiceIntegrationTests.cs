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

            string actualResult = new Bitlyfier().Shorten(longUrl).global_hash;
            
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
