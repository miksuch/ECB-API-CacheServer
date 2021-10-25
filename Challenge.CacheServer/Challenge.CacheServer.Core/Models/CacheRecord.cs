using System;

namespace Challenge.CacheServer.Core.Models
{
    public class CacheRecord
    {
        public int Id { get; set; }
        public int CurrencyPairId { get; set; }
        public CurrencyPair CurrencyPair { get; set; } = new CurrencyPair();
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
    }
}
