using InformationMinion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace InformationMinion.Services
{
    public class BittrexService
    {
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


        public HttpResponseModel CallApi(string url)
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
    }
}
