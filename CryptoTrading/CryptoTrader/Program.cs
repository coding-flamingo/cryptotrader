using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using CryptoTrader.Services;
using CryptoTrader.Tests;
using CryptoTrader.Managers;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            if (test)
            {
                DirectoryInfo directory = new DirectoryInfo(@"D:\git\TestFiles\7-26");
                FileInfo[] files = directory.GetFiles("*.txt"); //Getting Text files
                foreach (var market in files)
                {
                    BittrexServiceTest _bittrexService = new BittrexServiceTest(market.FullName);
                    //Todo trading Magic
                    TradingLogicManager tradingManager = new TradingLogicManager(market.FullName, _bittrexService,  "PlanningToBuy");
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
                AKVService _aKVService = new AKVService();
                string _key = _aKVService.GetKeyVaultSecret(config["key"]);
                string _secret = _aKVService.GetKeyVaultSecret(config["secret"]);
                var marketName = "USD-BTC";
                BittrexService _bittrexService = new BittrexService(_key, _secret);
                TradingLogicManager tradingManager = new TradingLogicManager(marketName, _bittrexService,  "PlanningToBuy");

            }
        }
    }
}
