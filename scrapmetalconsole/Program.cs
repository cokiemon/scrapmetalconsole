﻿using System;
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
                await page.GoToAsync(aliPageAddress);

                // Wait for the page to load.
                //var resultsSelector = ".gsc-results .gsc-thumbnail-inside a.gs-title";
                var resultsSelector = ".product-price";
                await page.WaitForSelectorAsync(resultsSelector);





                var productSkuSelector = @"#root > div > div.product-main > div > div.product-info > div.product-sku";
                var productSkuHandle = await page.QuerySelectorAsync(productSkuSelector);
                ProductSku productSku = new ProductSku(productSkuHandle);
                await productSku.Parse();
            }
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
