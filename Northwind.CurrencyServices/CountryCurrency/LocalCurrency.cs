namespace Northwind.CurrencyServices.CountryCurrency
{
    using System.Text.Json.Serialization;

    public class LocalCurrency
    {
        public string CountryName { get; set; }

        public string CurrencyCode { get; set; }

        public string CurrencySymbol { get; set; }
    }
}
