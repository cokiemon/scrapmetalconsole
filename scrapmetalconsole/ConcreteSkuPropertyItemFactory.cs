using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public class ConcreteSkuPropertyItemFactory : SkuPropertyItemFactory
    {
        public override ISkuPropertyItem CreateSkuPropertyItem(string className)
        {
            switch (className)
            {
                case "sku-property-text":
                    return new TextSkuPropertyItem();

                case "sku-property-image":
                    return new ImageSkuPropertyItem();

                default:
                    throw new ApplicationException(
                        string.Format("Item '{0}' cannot be created.", className));
            }
        }

        public override async Task<ISkuPropertyItem> CreateSkuPropertyItem(ElementHandle elementHandle)
        {
            // Get the child div of li.sku-property-item which could be 
            // div.sku-property-text or div.sku-property-image
            var divHandle = await elementHandle.QuerySelectorAsync("div");
            string className = await divHandle.GetClassNameAsync();

            // For debug purposes only.
            Debug.WriteLine($"div className = {className}");

            switch (className)
            {
                case "sku-property-text":
                    TextSkuPropertyItem textSkuProperty = new TextSkuPropertyItem(className);
                    await textSkuProperty.Parse(elementHandle);

                    return textSkuProperty;

                case "sku-property-image":
                    ImageSkuPropertyItem imageSkuProperty = new ImageSkuPropertyItem(className);
                    await imageSkuProperty.Parse(elementHandle);

                    return imageSkuProperty;

                default:
                    throw new ApplicationException(
                        string.Format("Item '{0}' cannot be created.", className));
            }
        }
    }
}
