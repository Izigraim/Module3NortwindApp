using System;
using System.Data.Services.Client;
using System.Threading.Tasks;
using Northwind.CurrencyServices;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices.OData.ProductReports;
using NorthwindModel;

namespace ReportingApp
{
    /// <summary>
    /// Program class.
    /// </summary>
    public sealed class Program
    {
        private const string NorthwindServiceUrl = "https://services.odata.org/V3/Northwind/Northwind.svc";
        private const string CurrentProductsReport = "current-products";
        private const string MostExpensiveProductsReport = "most-expensive-products";
        private const string PriceLessThenProducts = "price-less-then-products";
        private const string PriceBetweenProducts = "price-between-products";
        private const string PriceAboveAverageProducts = "price-above-average-products";
        private const string UnitsInStockDeficit = "units-in-stock-deficit";
        private const string MostCheapProducts = "most-cheap-products";

        /// <summary>
        /// A program entry point.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                ShowHelp();
                return;
            }

            var reportName = args[0];

            if (string.Equals(reportName, CurrentProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowCurrentProducts();
                return;
            }
            else if (string.Equals(reportName, MostExpensiveProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && int.TryParse(args[1], out int count))
                {
                    await ShowMostExpensiveProducts(count);
                    return;
                }
            }
            else if (string.Equals(reportName, PriceLessThenProducts, StringComparison.InvariantCultureIgnoreCase))
            {
                args[1] = args[1].Replace('.', ',');

                if (args.Length > 1 && decimal.TryParse(args[1], out decimal price))
                {
                    await ShowPriceLessThenProducts(price);
                    return;
                }
            }
            else if (string.Equals(reportName, PriceBetweenProducts, StringComparison.InvariantCultureIgnoreCase))
            {
                args[1] = args[1].Replace('.', ',');
                args[2] = args[2].Replace('.', ',');

                if (args.Length > 2 && decimal.TryParse(args[1], out decimal lower) && decimal.TryParse(args[2], out decimal upper))
                {
                    await ShowPriceBetweenProducts(lower, upper);
                    return;
                }
            }
            else if (string.Equals(reportName, PriceAboveAverageProducts, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowPriceAboveAverageProducts();
                return;
            }
            else if (string.Equals(reportName, UnitsInStockDeficit, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowProductsInDeficit();
                return;
            }
            else if (string.Equals(reportName, MostCheapProducts, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && int.TryParse(args[1], out int count))
                {
                    await ShowMostCheapProducts(count);
                    return;
                }
            }
            else
            {
                var service = new CurrencyExchangeService("829c9b89f0e3e61d66d7df47f0d84783");
                await service.GetCurrencyExchangeRate("BYN", "USD");
                //var service = new CountryCurrencyService();
                //await service.GetLocalCurrencyByCountry("1");
                ShowHelp();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\tReportingApp.exe <report> <report-argument1> <report-argument2> ...");
            Console.WriteLine();
            Console.WriteLine("Reports:");
            Console.WriteLine($"\t{CurrentProductsReport}\t\tShows current products.");
            Console.WriteLine($"\t{MostExpensiveProductsReport}\t\tShows specified number of the most expensive products.");
            Console.WriteLine($"\t{PriceLessThenProducts}\t\tShows current products with a price less than one specified in the parameters.");
            Console.WriteLine($"\t{PriceBetweenProducts}\t\tShows current products with a price less that is between the lower and upper parameter.");
            Console.WriteLine($"\t{PriceAboveAverageProducts}\t\tShows current products with a price above average.");
            Console.WriteLine($"\t{UnitsInStockDeficit}\t\tShows current products that in dificit.");
            Console.WriteLine($"\t{MostCheapProducts}\t\tShows specified number of the most cheap products.");
        }

        private static async Task ShowCurrentProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetCurrentProductsReport();
            PrintProductReport("current products:", report);
        }

        private static async Task ShowMostExpensiveProducts(int count)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetMostExpensiveProductsReport(count);
            PrintProductReport($"{count} most expensive products:", report);
        }

        private static async Task ShowPriceLessThenProducts(decimal price)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetPriceLessThenProducts(price);
            PrintProductReport($"products with price less then {price}", report);
        }

        private static async Task ShowPriceBetweenProducts(decimal lower, decimal upper)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetPriceBetweenProducts(lower, upper);
            PrintProductReport($"products with price between {lower} and {upper}", report);
        }

        private static async Task ShowPriceAboveAverageProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetPriveAboveAverageProducts();
            PrintProductReport($"products with price above average", report);
        }

        private static async Task ShowProductsInDeficit()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetProductsInDeficit();
            PrintProductReport($"products in deficit", report);
        }

        private static async Task ShowMostCheapProducts(int count)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl));
            var report = await service.GetMostCheapProducts(count);
            PrintProductReport($"{count} most cheap products:", report);
        }

        private static void PrintProductReport(string header, ProductReport<ProductPrice> productReport)
        {
            Console.WriteLine($"Report - {header}");
            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine("{0}, {1}", reportLine.Name, reportLine.Price);
            }
        }
    }
}
