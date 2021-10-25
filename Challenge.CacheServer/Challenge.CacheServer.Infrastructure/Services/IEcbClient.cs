using Challenge.CacheServer.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Challenge.CacheServer.Infrastructure.Services
{
    public interface IEcbClient
    {
        public Task<List<ExchangeRateDto>> GetAsync(Dictionary<string, string> currencies, DateTime startPeriod, DateTime endPeriod);
    }
}
