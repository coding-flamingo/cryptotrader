using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Models
{
    public class CurrentPriceModel
    {
        public CurrentPriceModel()
        {

        }
        public CurrentPriceModel(double last, double bid, double ask)
        {
            Last = last;
            Bid = bid;
            Ask = ask;
            Avg = (last + bid + ask) / 3;
        }
        public CurrentPriceModel(double last, double bid, double ask, double avg)
        {
            Last = last;
            Bid = bid;
            Ask = ask;
            Avg = avg;
        }
        public double Last { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public double Avg { get; set; }
    }
    public class APIPriceModel
    {
        public bool success { get; set; }
        public CurrentPriceModel result { get; set; }
    }
}
