using CryptoTrader.Models;
using CryptoTrader.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Tests
{
    public class BittrexServiceTest : IBittrexService
    {
        Queue<CurrentPriceModel> _currentPrices = new Queue<CurrentPriceModel>();
        double _USDbalance;
        double _ALTBalance;
        int _sellcounter = 0;
        private readonly double startingcash = 1000;
        private readonly double _FirstPrice = 0;
        private readonly double _LastPrice;
        int sellCount;
        int buyCount;
        public bool exit = false;

        public BittrexServiceTest(string fileName)
        {
            sellCount = 0;
            buyCount = 0;
            CurrentPriceModel objectResponse;
            _USDbalance = startingcash;
            string line;
            System.IO.StreamReader file =
           new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                objectResponse = JsonConvert.DeserializeObject<CurrentPriceModel>(line);
                if (objectResponse.Avg > 0)
                {
                    _currentPrices.Enqueue(objectResponse);
                    if (_FirstPrice == 0)
                    {
                        _FirstPrice = objectResponse.Avg;
                    }
                    else
                    {
                        _LastPrice = objectResponse.Avg;
                    }
                }
            }
        }
        public CurrentPriceModel GetCurrentPrice(string market)
        {
            if (_currentPrices.Count > 0)
            {
                CurrentPriceModel result = _currentPrices.Dequeue();
                return result;
            }
            GetTotalSimulation(market);
            exit = true;
            return new CurrentPriceModel(0, 0, 0, 0);
        }

        public string Sell(double price, string market, double quantity)
        {
            sellCount++;
            _USDbalance = quantity * price;
            return Guid.NewGuid().ToString();
        }

        public string Buy(double price, string market, double quantity)
        {
            buyCount++;
            _ALTBalance = quantity;
            return Guid.NewGuid().ToString();
        }

        public double GetBalance(string coin)
        {
            if (coin.Contains("USD"))
            {
                return _USDbalance;
            }
            return _ALTBalance;
        }

        public bool GetOrderIsComplete(string orderNumber)
        {
            if (_sellcounter < 3)
            {
                _sellcounter++;
                return false;
            }
            _sellcounter = 0;
            return true;
        }

        public bool CancelOrder(string orderNumber)
        {
            return true;
        }

        private void GetTotalSimulation(string market)
        {
            Console.WriteLine("Market: " + market);
            Console.WriteLine("Sell Counter: " + sellCount + " Buy Counter: " + buyCount);
            Console.WriteLine("Starting Price: $" + _FirstPrice.ToString() + " Last Price: $" + _LastPrice.ToString() + " Market Percent Gain: " + (((_LastPrice - _FirstPrice) / _FirstPrice) * 100).ToString() + "%");
            Console.WriteLine("Starting Cash: $" + startingcash.ToString() + " Last Balance $" + _USDbalance.ToString() + " Algorithm Gain: " + (((_USDbalance - startingcash) / startingcash) * 100).ToString() + "%");
            Console.WriteLine("================================================================");
            Console.WriteLine("================================================================");
        }

    }
}
