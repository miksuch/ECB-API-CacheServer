using Challenge.CacheServer.Core.Interfaces;
using Challenge.CacheServer.Core.Models;
using Challenge.CacheServer.Infrastructure.Database;
using System;
using System.Linq;

namespace Challenge.CacheServer.Infrastructure.Repositories
{
    public class CacheRecordRepository : ICacheRecordRepository
    {
        private readonly CacheServerDbContext _dbContext;

        public CacheRecordRepository(CacheServerDbContext dbContext) =>
            (_dbContext) = (dbContext);

        public CacheRecord Get(CurrencyPair currency, DateTime startPeriod, DateTime endPeriod)
        {
             var result =_dbContext.CacheRecord
                .Where(cache =>
                cache.CurrencyPair.First == currency.First &&
                cache.CurrencyPair.Second == currency.Second &&
                cache.StartPeriod <= startPeriod &&
                cache.EndPeriod >= endPeriod)
                .FirstOrDefault();
            return result;
        }

        public void Update(CurrencyPair currency, DateTime startPeriod, DateTime endPeriod)
        {
            var affectedCache = _dbContext.CacheRecord
                .Where(cache =>
                cache.CurrencyPair.First == currency.First &&
                cache.CurrencyPair.Second == currency.Second &&
                !( cache.EndPeriod < startPeriod || cache.StartPeriod > endPeriod )
                );

            DateTime min = startPeriod, max = endPeriod;
            var updatedCache = new CacheRecord();
            updatedCache.CurrencyPair = null;
            if (affectedCache.Any())
            {
                updatedCache.CurrencyPairId = affectedCache.First().CurrencyPairId;
                foreach (var fragment in affectedCache)
                {
                    min = fragment.StartPeriod < min ? fragment.StartPeriod : min;
                    max = fragment.EndPeriod > max ? fragment.StartPeriod : max;
                }
                _dbContext.RemoveRange(affectedCache);
            }
            else
            {
                updatedCache.CurrencyPairId = _dbContext.CurrencyPair
                    .Where(pair =>
                    pair.First == currency.First &&
                    pair.Second == currency.Second)
                    .FirstOrDefault().Id;
                if (updatedCache.CurrencyPairId == default)
                    updatedCache.CurrencyPair = currency;
            }
            
            updatedCache.StartPeriod = min;
            updatedCache.EndPeriod = max;
            _dbContext.CacheRecord.Add(updatedCache);
            _dbContext.SaveChanges();

        }
    }
}
