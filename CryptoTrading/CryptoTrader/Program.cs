using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using CryptoTrader.Services;
namespace CryptoTrader
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            string _key = config["key"];
            string _secret = config["secret"];
            BittrexService _bittrexService = new BittrexService(_key, _secret);
            var balance = _bittrexService.GetBalance("BTC");
        }
    }
}
