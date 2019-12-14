using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Models
{
    public class BalanceAPIModel
    {
        public bool success { get; set; }
        public BalanceResultModel result { get; set; }
    }
    public class BalanceResultModel
    {
        public string Currency { get; set; }
        public double Balance { get; set; }
        public double Available { get; set; }
        public double Pending { get; set; }
        public string CryptoAddress { get; set; }
    }
}
