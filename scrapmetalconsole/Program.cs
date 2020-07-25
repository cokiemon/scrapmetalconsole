using System;
using System.Collections.Generic;
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

                ProductPrice price = await GetProductPrice(page);
                await GetSourceCountry(page);


                var productPriceOriginalSelector = ".product-price-original";
                var productPriceMarkSelector = ".product-price-mark";
                var productPriceOriginal = await page.QuerySelectorAsync(productPriceOriginalSelector);
                var productPriceMark = await productPriceOriginal.QuerySelectorAsync(productPriceMarkSelector);
                var content = await page.EvaluateFunctionAsync<string>("e => e.textContent", productPriceMark);


                var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);

                if (!args.Any(arg => arg == "auto-exit"))
                {
                    Console.ReadLine();
                }
            }
        }

        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello World!");

        //    var html = GetHtml("https://github.com/rflechner/ScrapySharp/blob/c867b8def04664f316b2441076347e105e311ca4/ScrapySharp.Tests/Html/Page1.htm");
        //    var s1 = html.CssSelect("span.login-box").ToArray();
        //    var s2 = html.CssSelect("span.pl-s").ToArray();
        //    var d = html.CssSelect("div").ToArray();
        //    int count = html.CssSelect("div.content").Count();
        //    int count2 = html.CssSelect(".content").Count();



        //    string website = "https://www.aliexpress.com/item/4000366605357.html";
        //    var html2 = GetHtml(website);
        //    var links = html.CssSelect("a");
        //    var product = html.CssSelect("div.product-price-original");
        //    var product2 = html.CssSelect("span.product-price-value");
        //    var spans = html.CssSelect("span.product-price-value").ToArray();

        //    var divs = html.CssSelect("div");
        //    var divs2 = html.CssSelect(".product-main");

        //}

        static HtmlNode GetHtml(string url)
        {
            WebPage webpage = _browser.NavigateToPage(new Uri(url));

            return webpage.Html;
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

        static async Task GetSourceCountry(Page page)
        {
            var listHandle = await page.QuerySelectorAsync(".sku-property-list");

            var script = @"Array.from(document.querySelectorAll('li')).map(a => a.class);";
            var urls = await page.EvaluateExpressionAsync<string[]>(script);
        }
    }
}
