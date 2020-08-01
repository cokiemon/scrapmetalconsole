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
            //Debug.WriteLine($"elementHandle innerHTML = {await elementHandle.GetInnerHtmlAsync()}");

            // Get the child img element of div.sku-property-image
            var imgHandle = await elementHandle.QuerySelectorAsync("img");
            ImageSource = await imgHandle.GetAttributeValueAsync("src");
            ImageTitle = await imgHandle.GetAttributeValueAsync("title");

            // For debug purposes only.
            Debug.WriteLine($"{ImageTitle}: {ImageSource}");

            return this;
        }

        #endregion
    }
}
