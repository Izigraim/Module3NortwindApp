using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using NorthwindProduct = NorthwindModel.Product;

namespace Northwind.ReportingServices.OData.ProductReports
{
    /// <summary>
    /// Represents a service that produces product-related reports.
    /// </summary>
    public class ProductReportService
    {
        private readonly NorthwindModel.NorthwindEntities entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="northwindServiceUri">An URL to Northwind OData service.</param>
        public ProductReportService(Uri northwindServiceUri)
        {
            this.entities = new NorthwindModel.NorthwindEntities(northwindServiceUri ?? throw new ArgumentNullException(nameof(northwindServiceUri)));
        }

        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetCurrentProductsReport()
        {
            var query = (DataServiceQuery<ProductPrice>)(
            from p in this.entities.Products
            where !p.Discontinued
            orderby p.ProductName descending
            select new ProductPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
            });

            return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products.
            Where(p => p.UnitPrice != null).
            OrderByDescending(p => p.UnitPrice.Value).
            Take(count).Select(p => new ProductPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
            });

            return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a product report with price less the specified one.
        /// </summary>
        /// <param name="price">Price.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceLessThenProducts(decimal? price)
        {
            var query = (DataServiceQuery<ProductPrice>)(
            from p in this.entities.Products
            where p.UnitPrice < price
            select new ProductPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
            });

            return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a product report with a price that is between the lower and upper parameters.
        /// </summary>
        /// <param name="lower">The lower limit of the price.</param>
        /// <param name="upper">The upper limit of the price.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriceBetweenProducts(decimal? lower, decimal? upper)
        {
            var query = (DataServiceQuery<ProductPrice>)(
            from p in this.entities.Products
            where p.UnitPrice > lower && p.UnitPrice < upper
            select new ProductPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
            });

            return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a product report with a price above the average.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetPriveAboveAverageProducts()
        {
            // ??????
            var query1 = (DataServiceQuery<ProductPrice>)(
            from p in this.entities.Products
            select new ProductPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
            });

            var items = await this.GetAllItems(query1).ConfigureAwait(false);

            // ???????
            decimal fullPrice = 0;
            foreach (var i in items)
            {
                fullPrice += i.Price;
            }

            decimal average = fullPrice / items.Count();

            // ???????
            var query = (DataServiceQuery<ProductPrice>)(
            from p in this.entities.Products
            where p.UnitPrice > average
            select new ProductPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
            });

            return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a product report with products in deficit.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetProductsInDeficit()
        {
            var query = (DataServiceQuery<ProductPrice>)(
            from p in this.entities.Products
            where p.UnitsInStock < p.UnitsOnOrder
            select new ProductPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
            });

            return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a product report with a specified number of most cheap products.
        /// </summary>
        /// <param name="count">Count of products.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostCheapProducts(int count)
        {
           var query = (DataServiceQuery<ProductPrice>)this.entities.Products.
           Where(p => p.UnitPrice != null).
           OrderBy(p => p.UnitPrice.Value).
           Take(count).Select(p => new ProductPrice
           {
               Name = p.ProductName,
               Price = p.UnitPrice ?? 0,
           });

           return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        public async Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(ICountryCurrencyService countryCurrencyService, ICurrencyExchangeService currencyExchangeService)
        {
            var query = (DataServiceQuery<ProductLocalPrice>)(
            from p in this.entities.Products
            where !p.Discontinued
            select new ProductLocalPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
                Country = p.Supplier.Country,
                LocalPrice = currencyExchangeService.GetCurrencyExchangeRate("USD", countryCurrencyService.GetLocalCurrencyByCountry(p.Supplier.Country).Result.CurrencyCode).Result * (p.UnitPrice ?? 0),
                CurrencySymbol = countryCurrencyService.GetLocalCurrencyByCountry(p.Supplier.Country).Result.CurrencySymbol,
            }) ;

            return await this.GetAllProductReport(query).ConfigureAwait(false);
        }

        private async Task<ProductReport<T>> GetAllProductReport<T>(DataServiceQuery<T> query)
        {
            var items = await this.GetAllItems(query).ConfigureAwait(false);
            return new ProductReport<T>(items);
        }

        private async Task<IEnumerable<T>> GetAllItems<T>(DataServiceQuery<T> query)
        {
            var items = new List<T>();
            var result = await Task<IEnumerable<T>>.Factory.FromAsync(query.BeginExecute(null, null), (ar) =>
            {
                return query.EndExecute(ar);
            }).ConfigureAwait(false) as QueryOperationResponse<T>;

            items.AddRange(result);

            while (result.GetContinuation() != null)
            {
                result = await Task<IEnumerable<T>>.Factory.FromAsync(this.entities.BeginExecute(result.GetContinuation(), null, null), (ar) =>
                {
                    return this.entities.EndExecute<T>(ar);
                }).ConfigureAwait(false) as QueryOperationResponse<T>;

                items.AddRange(result);
            }

            return items;
        }
    }
}
