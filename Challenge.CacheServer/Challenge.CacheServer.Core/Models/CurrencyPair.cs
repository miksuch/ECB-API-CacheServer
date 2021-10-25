using System.Collections.Generic;

namespace Challenge.CacheServer.Core.Models
{
    public class CurrencyPair
    {
        public int Id { get; set; }
        public string First { get; set; }
        public string Second { get; set; }

        public CurrencyPair() { }
        public CurrencyPair(string first, string second) =>
            (First, Second) = (first, second);

        public virtual IEnumerable<ExchangeRate> ExchangeRates { get; set; }
        public virtual IEnumerable<CacheRecord> CacheRecords { get; set; }

    }
}
