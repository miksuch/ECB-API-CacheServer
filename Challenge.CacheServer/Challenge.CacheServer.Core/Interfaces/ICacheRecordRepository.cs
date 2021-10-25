using Challenge.CacheServer.Core.Models;
using System;

namespace Challenge.CacheServer.Core.Interfaces
{
    public interface ICacheRecordRepository
    {
        public CacheRecord Get(CurrencyPair currency, DateTime startPeriod, DateTime endPeriod);
        public void Update(CurrencyPair currency, DateTime startPeriod, DateTime endPeriod);
    }
}
