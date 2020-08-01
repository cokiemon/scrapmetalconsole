using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public class ProductSku
    {
        #region Constructors

        public ProductSku()
        {
            Properties = new List<SkuProperty>();
        }

        public ProductSku(ElementHandle elementHandle)
        {
            ElementHandle = elementHandle;
            Properties = new List<SkuProperty>();
        }

        #endregion

        #region Public Proeprties

        public ElementHandle ElementHandle { get; private set; }

        public List<SkuProperty> Properties { get; set; }

        #endregion

        public async Task Parse()
        {
            if (ElementHandle == null)
            {
                throw new InvalidOperationException("ElementHandle property is null.");
            }

            // Try to check the current handle class name.
            string className = await getElementHandleClassName(ElementHandle);

            if (className != "product-sku")
            {
                string message = "The element handle should be at sku-property.";
                throw new InvalidOperationException(message);
            }

            // Selector for a list of sku-property. Child of div.product-sku
            //var asdasd = @"#root > div > div.product-main > div > div.product-info > div.product-sku > div > div:nth-child(1)";
            var skuPropertiesSelector = @"div > div.sku-property";
            var skuPropertiesHandle = await ElementHandle.QuerySelectorAllAsync(skuPropertiesSelector);

            foreach (ElementHandle handle in skuPropertiesHandle)
            {
                SkuProperty skuProperty = new SkuProperty(handle);
                await skuProperty.ParsePropertyTitle();
                await skuProperty.ParsePropertyList();

                Properties.Add(skuProperty);
            }
        }

        #region Private Methods

        private static async Task<string> getElementHandleClassName(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.className");
        }

        #endregion
    }
}
