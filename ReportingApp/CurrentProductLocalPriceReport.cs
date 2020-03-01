using System;
using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices;
using Northwind.ReportingServices.ProductReports;

namespace ReportingApp
{
    public class CurrentProductLocalPriceReport
    {
        private readonly IProductReportService productReportService;
        private readonly ICurrencyExchangeService currencyExchangeService;
        private readonly ICountryCurrencyService countryCurrencyService;

        public CurrentProductLocalPriceReport(IProductReportService productReportService, ICurrencyExchangeService currencyExchangeService, ICountryCurrencyService countryCurrencyService)
        {
            this.productReportService = productReportService ?? throw new ArgumentNullException(nameof(productReportService));
            this.currencyExchangeService = currencyExchangeService ?? throw new ArgumentNullException(nameof(currencyExchangeService));
            this.countryCurrencyService = countryCurrencyService ?? throw new ArgumentNullException(nameof(countryCurrencyService));
        }

        public async Task PrintReport()
        {
            var productReport = await this.productReportService.GetCurrentProductsWithLocalCurrencyReport(this.countryCurrencyService, this.currencyExchangeService);

            Console.WriteLine($"Report - curent products with local price:");
            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine("{0}, {1:F2}$, {2}, {3:F2}{4}", reportLine.Name, reportLine.Price, reportLine.Country, reportLine.LocalPrice, reportLine.CurrencySymbol);
            }
        }
    }
}
