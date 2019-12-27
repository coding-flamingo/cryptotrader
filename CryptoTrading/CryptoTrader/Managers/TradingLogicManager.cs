using CryptoTrader.Models;
using CryptoTrader.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Managers
{
    public class TradingLogicManager
    {
        private const string CURRENTLYBUYING = "CurrentlyBuying";
        private const string CURRENTLYSELLING = "CurrentlySeling";
        private const string PLANINGTOBUY = "PlanningToBuy";
        private const string PLANINGTOSELL = "PlaningToSell";
        private const double BUYTHRESHOLD = 4;
        private const double SELLTHRESHOLD = 3;
        private const double SELLCONSIDERMINTHRESH = .008;
        private const double BUYCONSIDERMINTHRESH = .01;
        private string _OrderNumber;
        private readonly IBittrexService _BitrexService;
        private readonly string _Market;
        CurrentPriceModel _CurrentPrice;
        private double _LastTransactionPrice;
        private string _OrderStatus;
        private double _CurrentBalance;
        List<CurrentPriceModel> _CurrentTrend;
        CurrentPriceModel _LastCurrentPrice;
        private double _TempTransactionPrice = 0;

        public TradingLogicManager(string market, IBittrexService bittrexService, string orderStatus)
        {
            _BitrexService = bittrexService;
            _Market = market;
            _CurrentPrice = _BitrexService.GetCurrentPrice(_Market);
            _LastTransactionPrice = _CurrentPrice.Last;
            _OrderStatus = orderStatus;
            if (_OrderStatus == PLANINGTOBUY)
            {
                //get base coin
                _CurrentBalance = _BitrexService.GetBalance(_Market.Split('-')[0]);
            }
            else
            {
                //get Alt Coin
                _CurrentBalance = _BitrexService.GetBalance(_Market.Split('-')[1]);
            }
            _CurrentTrend = new List<CurrentPriceModel>();
            _LastCurrentPrice = _CurrentPrice;
            for (int i = 0; i < 20; i++)
            {
                _CurrentTrend.Add(new CurrentPriceModel(_CurrentPrice.Last, _CurrentPrice.Bid, _CurrentPrice.Ask, _CurrentPrice.Avg));
            }
        }

        public void CheckOptions()
        {
            _CurrentPrice = _BitrexService.GetCurrentPrice(_Market);
            if (_CurrentPrice.Avg > 0 && _CurrentPrice.Avg != _LastCurrentPrice.Avg)
            {
                _CurrentTrend.Add(_CurrentPrice);
                _CurrentTrend.RemoveAt(0);
                if (_OrderStatus.Equals(CURRENTLYBUYING))
                {
                    CheckCurrentlyBuying();
                }
                else if (_OrderStatus.Equals(CURRENTLYSELLING))
                {
                    CheckCurrentlySelling();
                }
                else if (_OrderStatus.Equals(PLANINGTOBUY))
                {
                    CheckPlanningToBuy();
                }
                else
                {
                    CheckPlaningToSell();
                }
                _LastCurrentPrice = _CurrentPrice;
            }
            else
            {
                if (_OrderStatus.Equals(CURRENTLYBUYING))
                {
                    CheckCurrentlyBuying();
                }
                else if (_OrderStatus.Equals(CURRENTLYSELLING))
                {
                    CheckCurrentlySelling();
                }
            }
        }

        private void CheckPlaningToSell()
        {
            if ((_LastTransactionPrice - _CurrentPrice.Avg) / _LastTransactionPrice > 0.10)
            {
                Console.WriteLine("saving your life under 10% loss");
                SellStuff();
            }
            else if ((_CurrentPrice.Avg - _LastTransactionPrice) / _CurrentPrice.Avg < SELLCONSIDERMINTHRESH)//Price is too low we lose money
            {
                Console.WriteLine("too cheap to sell " + ((_CurrentPrice.Avg - _LastTransactionPrice) / _CurrentPrice.Avg).ToString());
            }
            else if (TryingtoSellCheckPreviewsTrend() > SELLTHRESHOLD)
            {
                SellStuff();
            }
            else
            {
                Console.WriteLine("Threshold not good enough to sell: " + TryingtoSellCheckPreviewsTrend().ToString());
            }
        }

        private void CheckCurrentlyBuying()
        {
            if (_BitrexService.GetOrderIsComplete(_OrderNumber))
            {
                _OrderStatus = PLANINGTOSELL;
                _CurrentBalance = _BitrexService.GetBalance(_Market.Split('-')[1]);
                Console.WriteLine("finished Buy Quantity: " + _CurrentBalance.ToString() + " Estimated usd worth: $" + (_CurrentBalance * _TempTransactionPrice).ToString());
            }
        }

        private void CheckCurrentlySelling()
        {
            if (_BitrexService.GetOrderIsComplete(_OrderNumber))
            {
                _OrderStatus = PLANINGTOBUY;
                _CurrentBalance = _BitrexService.GetBalance(_Market.Split('-')[0]);
                Console.WriteLine("finished sale Quantity: " + _CurrentBalance.ToString());
                _LastTransactionPrice = _TempTransactionPrice;
            }
        }

        private void CheckPlanningToBuy()
        {
            if ((_LastTransactionPrice - _CurrentPrice.Avg) / _LastTransactionPrice < BUYCONSIDERMINTHRESH)//price is too high we can't buy
            {
                Console.WriteLine("too expensive to buy " + ((_LastTransactionPrice - _CurrentPrice.Avg) / _LastTransactionPrice).ToString());
            }
            else if (TryingtoBuyCheckPreviewsTrend() > BUYTHRESHOLD)
            {
                BuyStuff();
            }
            else
            {
                Console.WriteLine("Threshold not good enough to buy: " + TryingtoBuyCheckPreviewsTrend().ToString());
            }

        }

        private void BuyStuff()
        {
            double quantity = (_CurrentBalance * .99) / ((_CurrentPrice.Bid + _CurrentPrice.Last) / 2);
            _OrderNumber = _BitrexService.Buy((_CurrentPrice.Bid + _CurrentPrice.Last) / 2, _Market, quantity);
            if (!_OrderNumber.Equals(string.Empty))
            {
                _OrderStatus = CURRENTLYBUYING;
                _TempTransactionPrice = (_CurrentPrice.Bid + _CurrentPrice.Last) / 2;
                Console.WriteLine("Buying Price: $" + _TempTransactionPrice.ToString());
            }
        }

        private void SellStuff()
        {
            _OrderNumber = _BitrexService.Sell((_CurrentPrice.Ask + _CurrentPrice.Last) / 2, _Market, _CurrentBalance);
            if (!_OrderNumber.Equals(string.Empty))
            {
                _TempTransactionPrice = (_CurrentPrice.Ask + _CurrentPrice.Last) / 2;
                _OrderStatus = CURRENTLYSELLING;
                Console.WriteLine("Selling Price: $" + _TempTransactionPrice.ToString());
            }
        }

        private double TryingtoBuyCheckPreviewsTrend()
        {
            int count = 0;
            for (int i = _CurrentTrend.Count - 1; i > 0; i--)
            {
                if (_CurrentTrend[i].Avg > _CurrentTrend[i - 1].Avg)
                {
                    count++;
                }
            }
            return count;
        }

        private double TryingtoSellCheckPreviewsTrend()
        {
            int count = 0;
            for (int i = _CurrentTrend.Count - 1; i > 0; i--)
            {
                if (_CurrentTrend[i].Avg < _CurrentTrend[i - 1].Avg)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
