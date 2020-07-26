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
                await page.GoToAsync(aliPageAddress);

                // Wait for the page to load.
                //var resultsSelector = ".gsc-results .gsc-thumbnail-inside a.gs-title";
                var resultsSelector = ".product-price";
                await page.WaitForSelectorAsync(resultsSelector);



                // Selector for a list of sku-property. Child of div.product-sku
                //var asdasd = @"#root > div > div.product-main > div > div.product-info > div.product-sku > div > div:nth-child(1)";
                var skuPropertiesSelector = @"#root > div > div.product-main > div > div.product-info > div.product-sku > div > div.sku-property";
                var skuPropertiesHandle = await page.QuerySelectorAllAsync(skuPropertiesSelector);

                foreach (ElementHandle handle in skuPropertiesHandle)
                {
                    await GetSkuProperty(page, handle);
                }

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

            //var script = @"Array.from(document.querySelectorAll('li')).map(a => a.class);";
            var script = @"Array.from(document.querySelectorAll('li'));";
            var urls = await page.EvaluateExpressionAsync<string[]>(script);

            var divs = await page.QuerySelectorAsync("li");
            var properties = await divs.GetPropertiesAsync();

            foreach (var property in properties)
            {
                var a = property.Value;
            }

            ElementHandle handle = await page.QuerySelectorAsync("li");

            var selector = @"#root > div > div.product-main > div > div.product-info > div.product-sku";
            var skuHandle = await page.QuerySelectorAsync(selector);

            //var xpath = "//*[@id="root"]/div/div[2]/div/div[2]/div[7]";
            var ambot = await listHandle.XPathAsync("");
        }

        private static async Task<string> getElementHandleClassName(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.className");
        }

        private static async Task<string> getElementHandleInnerHtml(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.innerHTML");
        }

        private static async Task<string> getElementHandleTextContent(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.textContent");
        }

        private static async Task GetPropertyList(Page page, ElementHandle handle)
        {
            // Selector for a list of sku-property-item. A child of 
            // ul.sku-property-list, which is a child of  div.sku-property.
            var skuPropertyItemsSelector = @"ul > li";
            var skuPropertyItemsHandle = await handle.QuerySelectorAllAsync(skuPropertyItemsSelector);

            foreach (var itemHandle in skuPropertyItemsHandle)
            {
                // For debug purposes only.
                //Debug.WriteLine($"itemHandle innerHTML = {await getElementHandleInnerHtml(itemHandle)}");



                SkuPropertyItemFactory factory = new ConcreteSkuPropertyItemFactory();
                ISkuPropertyItem item = await factory.CreateSkuPropertyItem(itemHandle);
            }
        }

        private static async Task GetSkuProperty(Page page, ElementHandle handle)
        {
            var skuTitleHandle = await handle.QuerySelectorAsync(".sku-title");
            string skuTitle = await page.EvaluateFunctionAsync<string>("e => e.textContent", skuTitleHandle);
            Debug.WriteLine("skuTitle = " + skuTitle);

            await GetPropertyList(page, handle);
        }
    }
}
