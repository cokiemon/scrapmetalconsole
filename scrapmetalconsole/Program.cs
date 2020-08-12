﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

//using HtmlAgilityPack;
//using ScrapySharp.Extensions;
//using ScrapySharp.Network;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    class Program
    {
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

                Store store = await StoreFactory.CreateStore(page);
                //logToConsole(store);
                ConsoleLogger.Log(store);
                Console.WriteLine();

                ProductInformation productInformation = await ProductInformation.CreateProductInformation(page);
                //logToConsole(productInformation);
                ConsoleLogger.Log(productInformation);
            }

            Environment.Exit(0);
        }

        private static void logToConsole(Store store)
        {
            // For debug purposes only.
            Debug.WriteLine($"Store Name: {store.Name}");
            Debug.WriteLine($"Store Link: {store.Link}");

            Console.WriteLine($"Store Name: {store.Name}");
            Console.WriteLine($"Store Link: {store.Link}");
        }

        private static void logToConsole(ProductInformation productInformation)
        {
            // For debug purposes only.
            Debug.WriteLine($"Title: {productInformation.TitleText}");
            Debug.WriteLine($"Url: {productInformation.Url}");

            Console.WriteLine($"Title: {productInformation.TitleText}");
            Console.WriteLine($"Url: {productInformation.Url}");

            if (productInformation.Sku != null)
            {
                foreach (var property in productInformation.Sku.Properties)
                {
                    Console.WriteLine($"{property.Title}");

                    foreach (var item in property.PropertyList)
                    {
                        if (item.ClassName == "sku-property-text")
                        {
                            TextSkuPropertyItem propertyItem = item as TextSkuPropertyItem;
                            Console.WriteLine($"\t{propertyItem.TextValue}");
                        }
                        else if (item.ClassName == "sku-property-image")
                        {
                            ImageSkuPropertyItem propertyItem = item as ImageSkuPropertyItem;
                            Console.WriteLine($"\t{propertyItem.ImageTitle}: {propertyItem.ImageSource}");
                        }
                    }
                }
            }
        }
    }
}
