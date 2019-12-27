using InformationMinion.Models;
using InformationMinion.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace InformationMinion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's get this party started!!");
            BittrexService _BittrexService = new BittrexService();
            List<MarketsNamesResuts> activeMarkets = _BittrexService.GetActiveMarkets();

            while (true)
            {
                foreach (var market in activeMarkets.Where(i => i.MarketName.Contains("USD")))
                {
                    try
                    {
                        var price = _BittrexService.GetCurrentPrice(market.MarketName);
                        using (StreamWriter sw = File.AppendText(market.MarketName + ".txt"))
                        {
                            var contentsToWriteToFile = JsonConvert.SerializeObject(price);
                            Console.WriteLine("Market: " + market.MarketName + " Prices: " + contentsToWriteToFile);
                            sw.WriteLine(contentsToWriteToFile);
                        }
                    }
                    catch
                    {

                    }
                    Thread.Sleep(250);
                }
            }
        }
    }
}
