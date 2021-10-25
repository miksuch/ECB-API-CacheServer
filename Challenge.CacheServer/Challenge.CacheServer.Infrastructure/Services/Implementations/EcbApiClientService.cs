using Challenge.CacheServer.Infrastructure.Models;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

namespace Challenge.CacheServer.Infrastructure.Services.Implementations
{
    public class EcbApiClientService : IEcbClient
    {
        private readonly ISdmxXmlReader _sdmxReader;

        public const String EcbUrl = "https://sdw-wsrest.ecb.europa.eu/service/data/EXR/";
        public const String StartPeriodParameterKey = "startPeriod";
        public const String EndPeriodParameterKey = "endPeriod";
        public const String DateFormat = "yyyy-MM-dd";

        public EcbApiClientService(ISdmxXmlReader sdmxXmlReader) =>
            _sdmxReader = sdmxXmlReader;

        public async Task<List<ExchangeRateDto>> GetAsync(Dictionary<string,string> currencies, DateTime startPeriod, DateTime endPeriod)
        {
            var result = new List<ExchangeRateDto>();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                MediaTypeWithQualityHeaderValue
                .Parse("application/vnd.sdmx.genericdata+xml;version=2.1"));

            var responses = new List<Task<HttpResponseMessage>>();
            foreach (var pair in currencies)
            {
                String key = $"D.{pair.Key}.{pair.Value}.SP00.A";
                String urlString = EcbUrl + key;

                var dictionary = new Dictionary<string, string>();

                dictionary.Add(StartPeriodParameterKey, startPeriod.ToString(DateFormat));
                dictionary.Add(EndPeriodParameterKey, endPeriod.ToString(DateFormat));

                var uri = new Uri(QueryHelpers.AddQueryString(urlString, dictionary));
                responses.Add(client.GetAsync(uri));
            }

            var convertedResponses = new List<Task<ExchangeRateDto>>();
            while(responses.Any())
            {
                var finishedTask = await Task.WhenAny(responses);
                responses.Remove(finishedTask);
                var response = await finishedTask;
                try
                {
                    var exchangeRatesFragment = _sdmxReader.GetExchangeRates(await response.Content.ReadAsStreamAsync());
                    result.AddRange(exchangeRatesFragment);
                }
                catch( XmlException )
                {
                    throw new EcbServerMessageException(await response.Content.ReadAsStringAsync(), response.StatusCode);
                }
            }

            return result;
        }
    }
}
