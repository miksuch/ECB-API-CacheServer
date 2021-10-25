using Challenge.CacheServer.Infrastructure.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Challenge.CacheServer.Tests.Infrastructure.Services
{
    public class EcbTestData : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[]
            {
                new Dictionary<string, string>
                {
                    { "CHF", "EUR" }
                },
                new DateTime(2020,1,2),
                new DateTime(2020,1,3),
                new List<ExchangeRateDto>
                {
                    new ExchangeRateDto
                    {
                        CurrencyFirst = "CHF",
                        CurrencySecond = "EUR",
                        DateValues = new List<Tuple<DateTime?,double?>>
                        {
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,2), 1.0865 ),
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,3), 1.084 )
                        }
                    }
                }
            },
            new object[]
            {
                new Dictionary<string, string>
                {
                    { "PLN", "EUR" },
                    { "CHF", "EUR" }
                },
                new DateTime(2020,1,1),
                new DateTime(2020,1,7),
                new List<ExchangeRateDto>
                {
                    new ExchangeRateDto
                    {
                        CurrencyFirst = "PLN",
                        CurrencySecond = "EUR",
                        DateValues = new List<Tuple<DateTime?,double?>>
                        {
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,2), 4.2544 ),
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,3), 4.2493 ),
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,6), 4.2415 ),
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,7), 4.2457 )
                        }
                    },
                    new ExchangeRateDto
                    {
                        CurrencyFirst = "CHF",
                        CurrencySecond = "EUR",
                        DateValues = new List<Tuple<DateTime?,double?>>
                        {
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,2), 1.0865 ),
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,3), 1.084 ),
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,6), 1.085 ),
                            new Tuple<DateTime?,double?>( new DateTime(2020,1,7), 1.085 )
                        }
                    }
                }
            }
        };

        public IEnumerator<object[]> GetEnumerator() =>
            _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}