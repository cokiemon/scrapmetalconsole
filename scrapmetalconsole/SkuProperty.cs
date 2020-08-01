using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public class SkuProperty
    {
        #region Constructors

        public SkuProperty()
        {
            PropertyList = new List<ISkuPropertyItem>();
        }

        public SkuProperty(ElementHandle elementHandle)
        {
            ElementHandle = elementHandle;
            PropertyList = new List<ISkuPropertyItem>();
        }

        #endregion

        #region Public Proeprties

        public ElementHandle ElementHandle { get; private set; }

        public string Title { get; set; }

        public List<ISkuPropertyItem> PropertyList { get; set; }

        #endregion

        #region Public Methods

        public async Task<string> ParsePropertyTitle()
        {
            if (ElementHandle == null)
            {
                throw new InvalidOperationException("ElementHandle property is null.");
            }

            // Try to check the current handle class name.
            string className = await getElementHandleClassName(ElementHandle);

            if (className != "sku-property")
            {
                string message = "The element handle should be at sku-property.";
                throw new InvalidOperationException(message);
            }

            // ANother check for the classname.

            var skuTitleHandle = await ElementHandle.QuerySelectorAsync(".sku-title");
            Title = await skuTitleHandle.EvaluateFunctionAsync<string>("e => e.textContent");

            return Title;
        }

        public async Task<List<ISkuPropertyItem>> ParsePropertyList()
        {
            if (ElementHandle == null)
            {
                throw new InvalidOperationException("ElementHandle property is null.");
            }

            // Try to check the current handle class name.
            string className = await getElementHandleClassName(ElementHandle);

            if (className != "sku-property")
            {
                string message = "The element handle should be at sku-property.";
                throw new InvalidOperationException(message);
            }

            List<ISkuPropertyItem> skuPropertyList = new List<ISkuPropertyItem>();

            // Selector for a list of sku-property-item. A child of 
            // ul.sku-property-list, which is a child of  div.sku-property.
            var skuPropertyItemsSelector = @"ul > li";
            var skuPropertyItemsHandle = await ElementHandle.QuerySelectorAllAsync(skuPropertyItemsSelector);

            foreach (var itemHandle in skuPropertyItemsHandle)
            {
                // For debug purposes only.
                //Debug.WriteLine($"itemHandle innerHTML = {await getElementHandleInnerHtml(itemHandle)}");

                SkuPropertyItemFactory factory = new ConcreteSkuPropertyItemFactory();
                skuPropertyList.Add(await factory.CreateSkuPropertyItem(itemHandle));
            }

            PropertyList = skuPropertyList;

            return PropertyList;
        }

        #endregion

        #region Private Methods

        private static async Task<string> getElementHandleClassName(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.className");
        }

        #endregion
    }
}
