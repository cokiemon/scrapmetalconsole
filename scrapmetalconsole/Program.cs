using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    class Program
    {
        public class Data
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public override string ToString() => $"Title: {Title} \nURL: {Url}";
        }

        static ScrapingBrowser _browser = new ScrapingBrowser();

        public static async Task Main(string[] args)
        {
            var options = new LaunchOptions
            {
                Headless = true
            };

            Console.WriteLine("Downloading chromium");
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            Console.WriteLine("Navigating google");
            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                string aliPageAddress = "https://www.aliexpress.com/item/4000366605357.html";

                try
                {
                    await page.GoToAsync(aliPageAddress);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Environment.Exit(1);
                }

                Store store = await ParseStore(page);
                // For debug purposes only.
                Debug.WriteLine($"Store Name: {store.Name}");
                Debug.WriteLine($"Store Link: {store.Link}");

                // Wait for the page to load.
                var productSkuSelector = @"#root > div > div.product-main > div > div.product-info > div.product-sku";
                await page.WaitForSelectorAsync(productSkuSelector);

                Product product = new Product();
                product.Title = "S";

                var productSkuHandle = await page.QuerySelectorAsync(productSkuSelector);
                ProductSku productSku = new ProductSku(productSkuHandle);
                await productSku.Parse();
            }

            Environment.Exit(0);
        }

        static async Task<Store> ParseStore(Page page)
        {
            // Wait for element to load.
            var storeNameSelector = @"#store-info-wrap > div.store-container > h3";
            await page.WaitForSelectorAsync(storeNameSelector);

            // Get store name selector.
            var storeNameHandle = await page.QuerySelectorAsync(storeNameSelector);

            // For debug purposes only.
            //Debug.WriteLine($"elementHandle innerHTML = {await storeNameHandle.GetInnerHtmlAsync()}");

            // Get anchor.
            var anchorHandle = await storeNameHandle.QuerySelectorAsync("a");

            Store store = new Store();
            store.Name = await anchorHandle.GetTextContentAsync();
            store.Link = await anchorHandle.GetAttributeValueAsync("href");

            return store;
        }

        static async Task<ProductPrice> GetProductPrice(Page page)
        {
            ProductPrice price = new ProductPrice();
            var productPriceHandle = await page.QuerySelectorAsync(".product-price");

            var priceCurrentHandle = await productPriceHandle.QuerySelectorAsync(".product-price-current");
            var priceOriginalHandle = await productPriceHandle.QuerySelectorAsync(".product-price-original");

            price.Value = await page.EvaluateFunctionAsync<string>("e => e.textContent",
                await priceCurrentHandle.QuerySelectorAsync(".product-price-value"));
            price.OriginalValue = await page.EvaluateFunctionAsync<string>("e => e.textContent",
                await priceOriginalHandle.QuerySelectorAsync(".product-price-value"));
            price.PriceMark = await page.EvaluateFunctionAsync<string>("e => e.textContent",
                await priceOriginalHandle.QuerySelectorAsync(".product-price-mark"));

            return price;
        }
    }
}
