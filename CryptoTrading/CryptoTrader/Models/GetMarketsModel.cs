using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Models
{
    public class GetMarketsModel
    {
        public bool success { get; set; }
        public List<MarketsNamesResuts> result { get; set; }
    }
    public class MarketsNamesResuts
    {
        public string MarketCurrency { get; set; }
        public string BaseCurrency { get; set; }
        public string MarketCurrencyLong { get; set; }
        public string BaseCurrencyLong { get; set; }
        public double MinTradeSize { get; set; }
        public string MarketName { get; set; }
        public bool IsActive { get; set; }
        public bool IsRestricted { get; set; }
        public DateTime Created { get; set; }
    }
}
