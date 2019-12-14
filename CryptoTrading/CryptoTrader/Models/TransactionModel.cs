using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Models
{
    public class TransactionModel
    {
        public bool success { get; set; }
        public TransactionResultModel result { get; set; }
    }
    public class TransactionResultModel
    {
        public string uuid { get; set; }
    }
}
