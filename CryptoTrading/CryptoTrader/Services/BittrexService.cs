using CryptoTrader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace CryptoTrader.Services
{
    public class BittrexService : IBittrexService
    {
        private readonly string _key;
        private readonly string _secret;
        public BittrexService (string key, string secret)
        {
            _key = key;
            _secret = secret;
        }
        public List<MarketsNamesResuts> GetActiveMarkets()
        {
            string url = "https://api.bittrex.com/api/v1.1/public/getmarkets";
            HttpResponseModel result = CallApi(url);
            if (result.Successfull)
            {
                GetMarketsModel objectResponse = JsonConvert.DeserializeObject<GetMarketsModel>(result.ResponseString);
                return objectResponse.result.Where(i => i.IsActive).ToList();
            }
            return new List<MarketsNamesResuts>();
        }
        public CurrentPriceModel GetCurrentPrice(string market)
        {
            string url = "https://api.bittrex.com/api/v1.1/public/getticker?market=" + market;
            HttpResponseModel result = CallApi(url);
            if (result.Successfull)
            {
                APIPriceModel objectResponse = JsonConvert.DeserializeObject<APIPriceModel>(result.ResponseString);
                objectResponse.result.Avg = (objectResponse.result.Bid + objectResponse.result.Ask + objectResponse.result.Last) / 3;
                return objectResponse.result;
            }
            return new CurrentPriceModel(0, 0, 0);
        }

        public string Buy(double price, string market, double quantity)
        {
            string url = "https://api.bittrex.com/api/v1.1/market/buylimit?apikey=" + _key + "&market=" + market + "&quantity=" + quantity + "&rate=" + price + "&nonce=" + GetNonce();
            HttpResponseModel result = CallSignedApi(url);
            if (result.Successfull)
            {
                TransactionModel objectResponse = JsonConvert.DeserializeObject<TransactionModel>(result.ResponseString);
                return objectResponse.result.uuid;
            }
            return "";
        }
        public string Sell(double price, string market, double quantity)
        {
            string url = "https://api.bittrex.com/api/v1.1/market/selllimit?apikey=" + _key + "&market=" + market + "&quantity=" + quantity + "&rate=" + price + "&nonce=" + GetNonce();
            HttpResponseModel result = CallSignedApi(url);
            if (result.Successfull)
            {
                TransactionModel objectResponse = JsonConvert.DeserializeObject<TransactionModel>(result.ResponseString);
                return objectResponse.result.uuid;
            }
            return "";
        }

        public bool GetOrderIsComplete(string orderNumber)
        {
            string url = "https://api.bittrex.com/api/v1.1/account/getorder?uuid=" + orderNumber + "&apikey=" + _key + "&nonce=" + GetNonce();
            HttpResponseModel result = CallSignedApi(url);
            if (result.Successfull)
            {
                try
                {
                    OrderStatusModel objectResponse = JsonConvert.DeserializeObject<OrderStatusModel>(result.ResponseString);
                    return objectResponse.result[0].Closed != null;
                }
                catch
                {
                    OrderStatusModel2 objectResponse = JsonConvert.DeserializeObject<OrderStatusModel2>(result.ResponseString);
                    return objectResponse.result.Closed != null;
                }
            }
            return false;
        }

        public bool CancelOrder(string orderNumber)
        {
            string url = "https://api.bittrex.com/api/v1.1/market/cancel?apikey=" + _key + "&uuid=" + orderNumber + "&nonce=" + GetNonce();
            HttpResponseModel result = CallSignedApi(url);
            return result.Successfull;
        }
        public double GetBalance(string coin)
        {
            string url = "https://api.bittrex.com/api/v1.1/account/getbalance?apikey=" + _key + "&currency=" + coin + "&nonce=" + GetNonce();
            HttpResponseModel result = CallSignedApi(url);
            if (result.Successfull)
            {
                BalanceAPIModel objectResponse = JsonConvert.DeserializeObject<BalanceAPIModel>(result.ResponseString);
                return objectResponse.result.Balance;
            }
            return -999;
        }

        private HttpResponseModel CallApi(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            HttpResponseModel response = new HttpResponseModel();
            using (var client = new HttpClient())
            {
                try
                {
                    response.ResponseString = client.GetStringAsync(url).Result;
                    response.Successfull = true;
                }
                catch
                {
                    response.Successfull = false;
                    response.ResponseString = "error";
                }
                return response;
            }
        }
        
        private HttpResponseModel CallSignedApi(string url)
        {
            string signature = SignRequest(url);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            HttpResponseModel response = new HttpResponseModel();
            using (var client = new HttpClient())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(signature))
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("apisign", signature);
                    }
                    response.ResponseString = client.GetStringAsync(url).Result;
                    response.Successfull = true;
                }
                catch
                {
                    response.Successfull = false;
                    response.ResponseString = "error";
                }
                return response;
            }
        }

        private string GetNonce()
        {
            Random random = new Random();
            return random.Next(10000000, 999999999).ToString();
        }

        private string SignRequest(string message)
        {
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(_secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                byte[] hashmessage = hmacsha512.ComputeHash(messageBytes);
                return BitConverter.ToString(hashmessage).Replace("-", "");
            }
        }
    }
}
