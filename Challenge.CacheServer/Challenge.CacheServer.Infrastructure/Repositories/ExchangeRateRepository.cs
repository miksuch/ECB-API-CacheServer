using Challenge.CacheServer.Core.Interfaces;
using Challenge.CacheServer.Core.Models;
using Challenge.CacheServer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Challenge.CacheServer.Infrastructure.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private CacheServerDbContext _dbContext;

        public ExchangeRateRepository(CacheServerDbContext dbContext) =>
            (_dbContext) = (dbContext);

        public IEnumerable<ExchangeRate> Get(CurrencyPair currency, DateTime startPeriod, DateTime endPeriod )
        {
            var result = _dbContext.ExchangeRate
                .Where(rate =>
                rate.CurrencyPair.First == currency.First &&
                rate.CurrencyPair.Second == currency.Second &&
                rate.Date >= startPeriod && rate.Date <= endPeriod)
                .Include(rate => rate.CurrencyPair).AsNoTracking();
            return result;
        }

        public void Add(List<ExchangeRate> exchangeRates)
        {
            foreach (var exchangeRate in exchangeRates)
            {
                var dbPair = _dbContext.CurrencyPair
                    .Where(pair =>
                    exchangeRate.CurrencyPair.First == pair.First &&
                    exchangeRate.CurrencyPair.Second == pair.Second)
                    .AsNoTracking()
                    .FirstOrDefault();
                if (dbPair != null)
                {
                    exchangeRate.CurrencyPair = null;
                    exchangeRate.CurrencyPairId = dbPair.Id;

                    var duplicate = _dbContext
                        .ExchangeRate
                        .Where(rate =>
                       exchangeRate.CurrencyPairId == rate.CurrencyPairId &&
                       exchangeRate.Date == rate.Date)
                        .FirstOrDefault();

                    if (duplicate != null)
                        continue;
                }
                _dbContext.Add(exchangeRate);
            }
            _dbContext.SaveChanges();
        }

        private void Add(ExchangeRate exchangeRate)
        {
            var dbPair = _dbContext.CurrencyPair
                .Where(pair =>
                exchangeRate.CurrencyPair.First == pair.First &&
                exchangeRate.CurrencyPair.Second == pair.Second)
                .AsNoTracking()
                .FirstOrDefault();
            if (dbPair != null)
            {
                exchangeRate.CurrencyPair = null;
                exchangeRate.CurrencyPairId = dbPair.Id;
            }
            _dbContext.Add(exchangeRate);
        }
    }
}
