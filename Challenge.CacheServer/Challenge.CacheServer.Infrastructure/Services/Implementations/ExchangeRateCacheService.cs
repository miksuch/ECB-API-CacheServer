using Challenge.CacheServer.Core.Interfaces;
using Challenge.CacheServer.Core.Models;
using Challenge.CacheServer.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Challenge.CacheServer.Infrastructure.Services.Implementations
{
    public class ExchangeRateCacheService : IExchangeRateService
    {
        private readonly ICacheRecordRepository _cacheRecords;
        private readonly IExchangeRateRepository _exchangeRates;
        private readonly IEcbClient _ecbClient;

        public ExchangeRateCacheService(ICacheRecordRepository cacheRecordRepository, IExchangeRateRepository exchangeRateRepository, IEcbClient ecbClient) =>
            (_cacheRecords, _exchangeRates, _ecbClient) = (cacheRecordRepository, exchangeRateRepository, ecbClient);

        public async Task<List<ExchangeRateDto>> GetAsync(Dictionary<string,string> currencyPairs, DateTime startPeriod, DateTime endPeriod)
        {
            List<CurrencyPair> currencies = new List<CurrencyPair>();
            foreach (var pair in currencyPairs)
                currencies.Add( new CurrencyPair(pair.Key, pair.Value));

            List<ExchangeRateDto> result = new List<ExchangeRateDto>();

            for(int i = 0; i < currencies.Count; ++i)
            {
                if( _cacheRecords.Get(currencies[i], startPeriod, endPeriod) != null )
                {
                    var exchangeRates = _exchangeRates.Get(currencies[i], startPeriod, endPeriod);
                    if( exchangeRates.Any() )
                    {
                        ExchangeRateDto dto = new ExchangeRateDto();
                        var currencyPair = exchangeRates.First().CurrencyPair;
                        dto.CurrencyFirst = currencyPair.First;
                        dto.CurrencySecond = currencyPair.Second;

                        foreach (ExchangeRate rate in exchangeRates)
                            dto.DateValues.Add(new Tuple<DateTime?, double?>(rate.Date, rate.Value));

                        result.Add(dto);
                    }
                }
                else
                {
                    result = await _ecbClient.GetAsync(currencyPairs, startPeriod, endPeriod);
                    UpdateRates(result);
                    UpdateCache(result, startPeriod, endPeriod);
                }
            }

            return result;
        }

        private void UpdateRates(List<ExchangeRateDto> exchangeRates)
        {
            var domainRatesList = new List<ExchangeRate>();
            foreach(var rate in exchangeRates)
            {
                foreach(var dayValue in rate.DateValues)
                {
                    var domainRate = new ExchangeRate();
                    domainRate.CurrencyPair.First = rate.CurrencyFirst;
                    domainRate.CurrencyPair.Second = rate.CurrencySecond;
                    domainRate.Date = dayValue.Item1.Value;
                    domainRate.Value = dayValue.Item2.Value;
                    domainRatesList.Add(domainRate);
                }
            }
            _exchangeRates.Add(domainRatesList);

            return;
        }

        private void UpdateCache(List<ExchangeRateDto> exchangeRates, DateTime startPeriod, DateTime endPeriod)
        {
            foreach (var rate in exchangeRates)
            {
                var domainCurrency = new CurrencyPair(rate.CurrencyFirst, rate.CurrencySecond);
                _cacheRecords.Update(domainCurrency, startPeriod, endPeriod);
            }

            return;
        }
    }
}
