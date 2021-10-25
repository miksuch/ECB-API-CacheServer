using Challenge.CacheServer.Infrastructure.Models;
using Challenge.CacheServer.Infrastructure.Services;
using Challenge.CacheServer.Infrastructure.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Challenge.CacheServer.Tests.Infrastructure.Services
{
    public class EcbClientTests
    {
        [Theory]
        [ClassData(typeof(EcbTestData))]
        public async Task DataFetchedIsValid(
            Dictionary<string,string> currencies,
            DateTime startPeriod,
            DateTime endPeriod,
            List<ExchangeRateDto> expected)
        {
            ISdmxXmlReader sdmxXmlReader = new SdmxXmlReader();
            IEcbClient ecbClient = new EcbApiClientService(sdmxXmlReader);

            var actual = await ecbClient.GetAsync(
                currencies, startPeriod, endPeriod);

            Assert.Equal(expected.Count, actual.Count);

            foreach (var expectedFragment in expected)
            {
                Assert.Contains(actual, actualFragment =>
                    expectedFragment.CurrencyFirst == actualFragment.CurrencyFirst &&
                    expectedFragment.CurrencySecond == actualFragment.CurrencySecond &&
                    expectedFragment.DateValues.Count == actualFragment.DateValues.Count
               );

                // split for convenience
                Assert.Contains(actual, actualFragment =>
                    expectedFragment.DateValues.TrueForAll(expectedValue => actualFragment.DateValues.Contains(expectedValue)));
            }
        }
    }
}
