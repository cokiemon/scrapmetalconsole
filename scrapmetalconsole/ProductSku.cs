using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public class ProductSku
    {
        #region Private Fields

        private readonly string _localSelector = "div.product-sku";

        #endregion

        #region Constructors

        private ProductSku()
        {
            Properties = new List<SkuProperty>();
        }

        #endregion

        #region Public Proeprties

        //public ElementHandle ElementHandle { get; private set; }

        public List<SkuProperty> Properties { get; set; }

        #endregion

        #region Public Methods

        public static async Task<ProductSku> CreateProductSku(Page page)
        {
            // Wait for element to load.
            var selector = @"#root > div > div.product-main > div > div.product-info > div.product-sku";
            await page.WaitForSelectorAsync(selector);

            // Get store name selector.
            var elementHandle = await page.QuerySelectorAsync(selector);

            return await CreateProductSku(elementHandle);
        }

        public static async Task<ProductSku> CreateProductSku(ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException(nameof(elementHandle));
            }

            // Lets make sure we got the right ElementHandle.
            string className = await elementHandle.GetClassNameAsync();

            if (className != "product-sku")
            {
                string message = $"The argument {nameof(elementHandle)} does not have the right selector.";
                throw new ArgumentException(message);
            }

            // For debug purposes only.
            Debug.WriteLine($"storeNameHandle innerHTML = {await elementHandle.GetInnerHtmlAsync()}");

            // Selector for a list of sku-property. Child of div.product-sku
            var skuPropertiesSelector = @"div > div.sku-property";
            var skuPropertiesHandle = await elementHandle.QuerySelectorAllAsync(skuPropertiesSelector);

            ProductSku sku = new ProductSku();

            foreach (ElementHandle handle in skuPropertiesHandle)
            {
                SkuProperty skuProperty = new SkuProperty(handle);
                await skuProperty.ParsePropertyTitle();
                await skuProperty.ParsePropertyList();

                sku.Properties.Add(skuProperty);
            }

            return sku;
        }

        #endregion
    }
}
