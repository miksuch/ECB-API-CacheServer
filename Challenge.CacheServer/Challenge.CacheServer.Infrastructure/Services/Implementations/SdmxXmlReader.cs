using Challenge.CacheServer.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Challenge.CacheServer.Infrastructure.Services.Implementations
{
    public class SdmxXmlReader : ISdmxXmlReader
    {
        public List<ExchangeRateDto> GetExchangeRates(Stream stream)
        {
            var result = new List<ExchangeRateDto>();
            var exchangeRate = new ExchangeRateDto();
            using (XmlReader reader = XmlReader.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.Name == "generic:SeriesKey")
                    {
                        var pair = GetCurrencyPairFromNode(reader);
                        exchangeRate.CurrencyFirst = pair.Item1;
                        exchangeRate.CurrencySecond = pair.Item2;
                    }
                    else if (reader.Name == "generic:Obs")
                    {
                        exchangeRate.DateValues.Add(GetDayValueFromNode(reader));
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "generic:Series")
                    {
                        result.Add(exchangeRate);
                        exchangeRate = new ExchangeRateDto();
                    }
                }
            }
            return result;
        }

        private Tuple<DateTime?, double?> GetDayValueFromNode(XmlReader reader)
        {
            var endElementName = reader.Name;
            DateTime? day = null;
            double? value = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "generic:ObsDimension")
                    {
                        day = DateTime.ParseExact(
                            reader.GetAttribute("value"),
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture);
                    }
                    else if (reader.Name == "generic:ObsValue")
                    {
                        value = double.Parse(reader.GetAttribute("value"));
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == endElementName)
                    break;
            }
            return new Tuple<DateTime?, double?>(day, value);
        }

        private Tuple<string,string> GetCurrencyPairFromNode(XmlReader reader)
        {
            var endElementName = reader.Name;
            string currency = "";
            string currencyDenominator = "";
            while (reader.Read())
            {
                if ((reader.GetAttribute("id") ?? "") == "CURRENCY")
                {
                    currency = reader.GetAttribute("value");
                }
                else if ((reader.GetAttribute("id") ?? "") == "CURRENCY_DENOM")
                {
                    currencyDenominator = reader.GetAttribute("value");
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == endElementName)
                    break;
            }
            return new Tuple<string, string>(currency,currencyDenominator);
        }
    }
}
