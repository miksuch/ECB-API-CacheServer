using System;
using System.Collections.Generic;

namespace Challenge.CacheServer.Infrastructure.Models
{
    public class ExchangeRateDto
    {
        public string CurrencyFirst { get; set; }
        public string CurrencySecond { get; set; }
        public List<Tuple<DateTime?, double?>> DateValues { get; set; } = new List<Tuple<DateTime?, double?>>();
    }
}
