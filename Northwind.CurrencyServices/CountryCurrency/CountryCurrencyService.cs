namespace Northwind.CurrencyServices.CountryCurrency
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CountryCurrencyService
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName)
        {
            var msg = Client.GetStringAsync($"https://restcountries.eu/rest/v2/name/{countryName}");
            var msg1 = await msg;

            LocalCurrency localCurrency = new LocalCurrency();

            using (JsonDocument jsonDocument = JsonDocument.Parse(msg1))
            {
                JsonElement root = jsonDocument.RootElement[0];
                JsonElement quotes = root.GetProperty("name");

                localCurrency.CountryName = quotes.GetString();
                localCurrency.CurrencyCode = root.GetProperty("currencies")[0].GetProperty("code").GetString();
                localCurrency.CurrencySymbol = root.GetProperty("currencies")[0].GetProperty("symbol").GetString();
            }

            return localCurrency;
        }
    }
}
