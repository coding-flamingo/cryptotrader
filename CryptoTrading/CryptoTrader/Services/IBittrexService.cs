using CryptoTrader.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Services
{
    public interface IBittrexService
    {
        bool GetOrderIsComplete(string orderNumber);
        bool CancelOrder(string orderNumber);
        double GetBalance(string coin);
        CurrentPriceModel GetCurrentPrice(string market);
        string Buy(double price, string market, double quantity);
        string Sell(double price, string market, double quantity);
    }
}
