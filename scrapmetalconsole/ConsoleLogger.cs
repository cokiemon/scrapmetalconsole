using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace scrapmetalconsole
{
    public static class ConsoleLogger
    {
        public static void Log(Store store)
        {
            // For debug purposes only.
            Debug.WriteLine($"Store Name: {store.Name}");
            Debug.WriteLine($"Store Link: {store.Link}");

            Console.WriteLine($"Store Name: {store.Name}");
            Console.WriteLine($"Store Link: {store.Link}");
        }

        public static void Log(ProductInformation productInformation)
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
