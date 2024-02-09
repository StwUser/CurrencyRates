using ExchangeRates.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeRates.Services
{
    public static class CurrencyRateService
    {
        public static async Task<List<CurrencyRate>> GetCurrencyRatesByPeriod(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var httpClient = new HttpClient();
                var collection = new List<CurrencyRate>();

                while (startDate <= endDate) 
                {
                    var apiUrl = $"https://api.nbrb.by/exrates/rates?ondate={startDate:yyy-MM-dd}&periodicity=0";

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl))
                    {
                        using (HttpResponseMessage response = await httpClient.SendAsync(request))
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var rates = (List<Rate>)JsonConvert.DeserializeObject(content, typeof(List<Rate>));

                            collection.AddRange(ConvertRatesToCurrencyRates(rates));
                        };
                    };

                    startDate = startDate.Value.AddDays(1);
                }
                var jsonCollection = JsonConvert.SerializeObject(collection, Formatting.Indented);
                string docPath = Environment.CurrentDirectory;
                File.WriteAllText(System.IO.Path.Combine(docPath, "CurrencyRates.json"), jsonCollection);

                return collection;
            }
            catch (Exception ex)
            {
                string docPath = Environment.CurrentDirectory;
                string[] text = { $"Error was occured - {DateTime.Now:yyyy-MM-dd}", $"{ex.Message}" };
                File.AppendAllLines(System.IO.Path.Combine(docPath, "CurrencyRatesServiceErrorLog.txt"), text);
            }
            return new List<CurrencyRate>();
        }

        public static Task FillCurrencyRatesFromJson(ObservableCollection<CurrencyRate> currencyRates)
        {
            try
            {
                if (File.Exists("CurrencyRates.json"))
                {
                    var jsonCollection = File.ReadAllText("CurrencyRates.json");
                    var rates = (List<CurrencyRate>)JsonConvert.DeserializeObject(jsonCollection, typeof(List<CurrencyRate>));

                    foreach (var rate in rates)
                    {
                        currencyRates.Add(rate);
                    }
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                string docPath = Environment.CurrentDirectory;
                string[] text = { $"Error was occured - {DateTime.Now:yyyy-MM-dd}", $"{ex.Message}" };
                File.AppendAllLines(System.IO.Path.Combine(docPath, "CurrencyRatesServiceErrorLog.txt"), text);
            }
            return Task.FromResult(false);
        }

        public static void SaveChangesInCollection(ObservableCollection<CurrencyRate> currencyRates)
        {
            var jsonCollection = JsonConvert.SerializeObject(currencyRates, Formatting.Indented);
            string docPath = Environment.CurrentDirectory;
            File.WriteAllText(System.IO.Path.Combine(docPath, "CurrencyRates.json"), jsonCollection);
        }

        private static List<CurrencyRate> ConvertRatesToCurrencyRates(List<Rate> rates)
        {
            var result = rates.Select(r => new CurrencyRate(r.Date, r.Cur_Abbreviation, r.Cur_Name, (double)r.Cur_OfficialRate)).ToList();
            return result;
        }
    }
}
