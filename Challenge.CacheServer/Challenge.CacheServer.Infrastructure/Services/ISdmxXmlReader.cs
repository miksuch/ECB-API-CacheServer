using Challenge.CacheServer.Infrastructure.Models;
using System.Collections.Generic;
using System.IO;

namespace Challenge.CacheServer.Infrastructure.Services
{
    public interface ISdmxXmlReader
    {
        public List<ExchangeRateDto> GetExchangeRates(Stream stream);
    }
}
