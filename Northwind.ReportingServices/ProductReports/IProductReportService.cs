using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;

namespace Northwind.ReportingServices.ProductReports
{
    public interface IProductReportService
    {
        Task<ProductReport<ProductPrice>> GetCurrentProductsReport();

        Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count);

        Task<ProductReport<ProductPrice>> GetPriceLessThenProducts(decimal? price);

        Task<ProductReport<ProductPrice>> GetPriceBetweenProducts(decimal? lower, decimal? upper);

        Task<ProductReport<ProductPrice>> GetPriveAboveAverageProducts();

        Task<ProductReport<ProductPrice>> GetProductsInDeficit();

        Task<ProductReport<ProductPrice>> GetMostCheapProducts(int count);

        Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(ICountryCurrencyService countryCurrencyService, ICurrencyExchangeService currencyExchangeService);

    }
}
