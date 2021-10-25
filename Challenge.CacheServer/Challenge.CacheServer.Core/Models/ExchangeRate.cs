using System;

namespace Challenge.CacheServer.Core.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public int CurrencyPairId { get; set; }
        public CurrencyPair CurrencyPair { get; set; } = new CurrencyPair();
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}
