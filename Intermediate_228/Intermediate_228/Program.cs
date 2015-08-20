using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Intermediate_228
{
    class Program
    {
        static void Main(string[] args)
        {
            var numberOfArguments = args.Count();

            switch (numberOfArguments)
            {
                case 2:
                    args[0] = args[0].ToLower();
                    args[1] = args[1].ToUpper();

                    if (IsValidInput(args[0], args[1]))
                    {
                        OutputTradeInformation(args[0], args[1]);
                    }
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }

        private static void OutputTradeInformation(string inputMarket, string inputCurrency)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(ConfigurationManager.AppSettings["BitcoinChartsBaseUri"])
            };

            var request = new RestRequest
            {
                Resource = "v1/trades.csv"
            };

            request.AddQueryParameter("symbol", string.Concat(inputMarket, inputCurrency));

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content.Length > 0)
            {
                var latestTradeData = response.Content.Substring(0, response.Content.IndexOf("\n"));
                var latestTradePice = Convert.ToDecimal(latestTradeData.Split(',')[1]);
                Console.WriteLine(latestTradePice.ToString("F2"));
            }
            else
            {
                Console.WriteLine("No data.");
            }
        }

        private static bool IsValidInput(string inputMarket, string inputCurrency)
        {
            var isValidInput = true;

            var bitcoinMarkets = new List<string> { "bitfinex", "bitstamp", "btce", "itbit", "anxhk", "hitbtc", "kraken", "bitkonan", "bitbay", "rock", "cbx", "cotr", "vcx" };
            var currencies = new List<string> { "KRW", "NMC", "IDR", "RON", "ARS", "AUD", "BGN", "BRL", "BTC", "CAD", "CHF", "CLP", "CNY", "CZK", "DKK", "EUR", "GAU", "GBP", "HKD", "HUF", "ILS", "INR", "JPY", "LTC", "MXN", "NOK", "NZD", "PEN", "PLN", "RUB", "SAR", "SEK", "SGD", "SLL", "THB", "UAH", "USD", "XRP", "ZAR" };

            if (!bitcoinMarkets.Contains(inputMarket))
            {
                Console.WriteLine("Valid market options are:");
                foreach (var market in bitcoinMarkets)
                {
                    Console.Write(market + " ");
                }
                Console.WriteLine();
                isValidInput = false;
            }

            if (!currencies.Contains(inputCurrency))
            {
                Console.WriteLine("Valid currency options are:");
                foreach (var currency in currencies)
                {
                    Console.Write(currency + " ");
                }
                Console.WriteLine();
                isValidInput = false;
            }

            return isValidInput;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " {market} {currency}");
            IsValidInput(string.Empty, string.Empty);
        }
    }
}
