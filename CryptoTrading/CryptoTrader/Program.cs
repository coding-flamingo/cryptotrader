using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using CryptoTrader.Services;
using CryptoTrader.Tests;
using CryptoTrader.Managers;

namespace CryptoTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            bool test = true;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string _key = config["key"];
            string _secret = config["secret"];
            if (test)
            {
                DirectoryInfo directory = new DirectoryInfo(@"D:\git\TestFiles\7-26");
                FileInfo[] files = directory.GetFiles("*.txt"); //Getting Text files
                foreach (var market in files)
                {
                    BittrexServiceTest _bittrexService = new BittrexServiceTest(market.FullName);
                    //Todo trading Magic
                    TradingLogicManager tradingManager = new TradingLogicManager(market.FullName, _bittrexService, 78, "PlanningToBuy");
                    while(_bittrexService.exit == false)
                    {
                        tradingManager.CheckOptions();
                    }
                }
                Console.WriteLine("Done :)");
                Console.ReadLine();

            }
            else
            {
                BittrexService _bittrexService = new BittrexService(_key, _secret);
            }
        }
    }
}
