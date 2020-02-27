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
            var msg = Client.GetStringAsync($"https://restcountries.eu/rest/v2/name/belarus");
            var msg1 = await msg;
            Console.WriteLine(msg1);

            return null;
        }
    }
}
