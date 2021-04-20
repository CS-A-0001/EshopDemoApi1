using CatalogDemo.API.Extensions;
using CatalogDemo.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CatalogDemo.API.Infrastructure
{
    public class CatalogContextSeed
    {
        public void Seed(CatalogContext context, IWebHostEnvironment env, IOptions<CatalogSettings> settings)
        {
            var useCustomizationData = settings.Value.UseCustomizationData;
            if (useCustomizationData && !context.Products.Any())
            {
                context.Products.AddRange(GetCatalogItemsFromFile(env.ContentRootPath, context));
                context.SaveChanges();
            }
        }

        IEnumerable<Product> GetCatalogItemsFromFile(string contentRootPath, CatalogContext context)
        {
            string csvFileCatalogItems = Path.Combine(contentRootPath, "Setup", "MOCK_DATA.csv");

            if (!File.Exists(csvFileCatalogItems))
            {
                return Enumerable.Empty<Product>();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "name", "imguri", "price", "description"};                
                csvheaders = GetHeaders(csvFileCatalogItems, requiredHeaders);
            }
            catch (Exception ex)
            {
                // In case of exception seed won't be initialized
                return Enumerable.Empty<Product>();
            }
           
            return File.ReadAllLines(csvFileCatalogItems)
                        .Skip(1) // skip header row
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .SelectTry(column => CreateProduct(column, csvheaders))
                        .OnCaughtException(ex => {  return null; })
                        .Where(x => x != null);
        }

        Product CreateProduct(string[] column, string[] headers)
        {
            string priceString = column[Array.IndexOf(headers, "price")].Trim('"').Trim();
            if (!decimal.TryParse(priceString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var price))
            {
                throw new Exception($"price={priceString}is not a valid decimal number");
            }
            var product = new Product()
            {                                                
                Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
                ImgUri = column[Array.IndexOf(headers, "imguri")].Trim('"').Trim(),
                Price = price,
                Description = column[Array.IndexOf(headers, "description")].Trim('"').Trim(),
            };

            return product;
        }

        string[] GetHeaders(string csvfile, string[] requiredHeaders)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }           

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }
    }
}
