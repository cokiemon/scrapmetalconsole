using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public class ImageSkuPropertyItem : ISkuPropertyItem
    {
        #region Constructors

        public ImageSkuPropertyItem()
        {

        }

        public ImageSkuPropertyItem(string className)
        {
            ClassName = className;
        }

        #endregion

        #region Public Properties

        public string ClassName { get; set; }

        public bool IsSelected { get; set; }

        public string ImageSource { get; set; }

        public string ImageTitle { get; set; }

        #endregion

        #region Public Methods

        public async Task<ISkuPropertyItem> Parse(ElementHandle elementHandle)
        {
            // For debug purposes only.
            //Debug.WriteLine($"divHandle innerHTML = {await getElementHandleInnerHtml(elementHandle)}");

            // Get the child img element of div.sku-property-image
            var imgHandle = await elementHandle.QuerySelectorAsync("img");
            ImageSource = await getElementHandleAttributeValue(imgHandle, "src");
            ImageTitle = await getElementHandleAttributeValue(imgHandle, "title");

            // For debug purposes only.
            Debug.WriteLine($"{ImageTitle}: {ImageSource}");

            return this;
        }

        #endregion

        #region Private Methods

        private static async Task<string> getElementHandleInnerHtml(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.innerHTML");
        }

        private static async Task<string> getElementHandleTextContent(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.textContent");
        }

        private static async Task<string> getElementHandleClassName(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.className");
        }

        private static async Task<string> getElementHandleAttributeValue(ElementHandle handle)
        {
            return await handle.EvaluateFunctionAsync<string>("e => e.getAttribute('src')");
        }

        private static async Task<string> getElementHandleAttributeValue(ElementHandle handle, string attribute)
        {
            string script = "e => e.getAttribute('')";

            return await handle.EvaluateFunctionAsync<string>(script.Insert(21, attribute));
        }

        #endregion
    }
}
