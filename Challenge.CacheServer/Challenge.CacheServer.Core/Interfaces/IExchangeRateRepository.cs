using Challenge.CacheServer.Core.Models;
using System;
using System.Collections.Generic;

namespace Challenge.CacheServer.Core.Interfaces
{
    public interface IExchangeRateRepository
    {
        public IEnumerable<ExchangeRate> Get(CurrencyPair currency, DateTime startPeriod, DateTime endPeriod);
        public void Add(List<ExchangeRate> exchangeRates);
    }
}
