using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public class TextSkuPropertyItem : ISkuPropertyItem
    {
        #region Constructors

        public TextSkuPropertyItem()
        {

        }

        public TextSkuPropertyItem(string className)
        {
            ClassName = className;
        }

        #endregion

        #region Public Properties

        public string ClassName { get; set; }

        public bool IsSelected { get; set; }

        public string TextValue { get; set; }

        #endregion

        #region Public Methods

        public async Task<ISkuPropertyItem> Parse(ElementHandle elementHandle)
        {
            // For debug purposes only.
            //Debug.WriteLine($"divHandle innerHTML = {await getElementHandleInnerHtml(elementHandle)}");

            TextValue = await getElementHandleTextContent(elementHandle);

            // For debug purposes only.
            Debug.WriteLine(TextValue);

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

        #endregion
    }
}
