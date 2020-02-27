namespace Northwind.CurrencyServices.CurrencyExchange
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

    public class CurrencyExchangeService
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly string accessKey;

        public CurrencyExchangeService(string accesskey)
        {
            this.accessKey = !string.IsNullOrWhiteSpace(accesskey) ? accesskey : throw new ArgumentException("Access key is invalid.", nameof(accesskey));
        }

        public async Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency)
        {
            if (baseCurrency.Equals("USD", StringComparison.OrdinalIgnoreCase))
            {
                var msg = Client.GetStringAsync($"http://api.currencylayer.com/live?access_key={this.accessKey}&currencies={exchangeCurrency}");
                var msg1 = await msg;

                decimal rate = 0;

                using (JsonDocument jsonDocument = JsonDocument.Parse(msg1))
                {
                    JsonElement root = jsonDocument.RootElement;
                    JsonElement quotes = root.GetProperty("quotes");
                    rate = quotes.GetProperty(baseCurrency + exchangeCurrency).GetDecimal();
                }

                return rate;
            }
            else
            {
                var msg = Client.GetStringAsync($"http://api.currencylayer.com/live?access_key={this.accessKey}&currencies={baseCurrency}");
                var baseMSg = await msg;
                decimal baseRate = 0;

                using (JsonDocument jsonDocument = JsonDocument.Parse(baseMSg))
                {
                    JsonElement root = jsonDocument.RootElement;
                    JsonElement quotes = root.GetProperty("quotes");
                    baseRate = quotes.GetProperty("USD" + baseCurrency).GetDecimal();
                }

                msg = Client.GetStringAsync($"http://api.currencylayer.com/live?access_key={this.accessKey}&currencies={exchangeCurrency}");
                baseMSg = await msg;
                decimal exchangeRate = 0;

                using (JsonDocument jsonDocument = JsonDocument.Parse(baseMSg))
                {
                    JsonElement root = jsonDocument.RootElement;
                    JsonElement quotes = root.GetProperty("quotes");
                    exchangeRate = quotes.GetProperty("USD" + exchangeCurrency).GetDecimal();
                }

                return exchangeRate / baseRate;
            }
        }
    }
}
