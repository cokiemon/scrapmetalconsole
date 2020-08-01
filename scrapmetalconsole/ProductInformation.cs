using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public class ProductInformation
    {
        private ProductInformation()
        {

        }

        private ProductInformation(string titleText, string url)
        {
            TitleText = titleText;
            Url = url;
        }

        private ProductInformation(string titleText, string url, ProductSku sku)
        {
            TitleText = titleText;
            Url = url;
            Sku = sku;
        }

        #region Properties

        public string TitleText { get; set; }

        public string Url { get; set; }

        public ProductSku Sku { get; set; }

        #endregion

        #region Public Methods

        public static async Task<ProductInformation> CreateProductInformation(Page page)
        {
            // Wait for element to load.
            var selector = @"#root > div > div.product-main > div > div.product-info";
            await page.WaitForSelectorAsync(selector);

            // Get store name selector.
            var elementHandle = await page.QuerySelectorAsync(selector);

            return await CreateProductInformation(elementHandle);
        }

        public static async Task<ProductInformation> CreateProductInformation(ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException(nameof(elementHandle));
            }

            // Lets make sure we got the right ElementHandle.
            string className = await elementHandle.GetClassNameAsync();

            if (className != "product-info")
            {
                string message = $"The argument {nameof(elementHandle)} does not have the right selector.";
                throw new ArgumentException(message);
            }

            // For debug purposes only.
            Debug.WriteLine($"storeNameHandle innerHTML = {await elementHandle.GetInnerHtmlAsync()}");

            string titleText = await getProductTitleText(elementHandle);
            string productUrl = await getProductUrl(elementHandle);
            ProductSku sku = await getProductSku(elementHandle);

            return new ProductInformation(titleText, productUrl, sku);
        }

        //public static async Task<ProductTitle> CreateProductTitle(ElementHandle elementHandle)
        //{
        //    if (elementHandle == null)
        //    {
        //        throw new ArgumentNullException(nameof(elementHandle));
        //    }

        //    // Lets make sure we got the right ElementHandle.
        //    string className = await elementHandle.GetClassNameAsync();

        //    if (className != "product-title")
        //    {
        //        string message = $"The argument {nameof(elementHandle)} does not have the right selector.";
        //        throw new ArgumentException(message);
        //    }

        //    // For debug purposes only.
        //    Debug.WriteLine($"storeNameHandle innerHTML = {await elementHandle.GetInnerHtmlAsync()}");

        //    string titleText = await getProductTitleText(elementHandle);
        //    string productUrl = await getProductUrl(elementHandle);

        //    return new ProductTitle(titleText, productUrl);
        //}

        #endregion

        private static async Task<string> getProductTitleText(ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException(nameof(elementHandle));
            }

            // Get div.product-title > h1
            var selector = "div.product-title > h1";
            var titleTextHandle = await elementHandle.QuerySelectorAsync(selector);

            if (titleTextHandle != null)
            {
                return await titleTextHandle.GetTextContentAsync();
            }

            return null;
        }

        private static async Task<string> getProductUrl(ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException(nameof(elementHandle));
            }

            // Get meta.
            var metaHandles = await elementHandle.QuerySelectorAllAsync("meta");

            foreach (ElementHandle handle in metaHandles)
            {
                string itemprop = await handle.GetAttributeValueAsync("itemprop");

                if (itemprop == "url")
                {
                    return await handle.GetAttributeValueAsync("content");
                }
            }

            return null;
        }

        private static async Task<ProductSku> getProductSku(ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException(nameof(elementHandle));
            }

            // Get div.product-title > h1
            var selector = "div.product-sku";
            var handle = await elementHandle.QuerySelectorAsync(selector);

            return await ProductSku.CreateProductSku(handle);
        }
    }
}
