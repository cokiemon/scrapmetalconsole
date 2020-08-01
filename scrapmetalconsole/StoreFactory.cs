using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public static class StoreFactory
    {
        public static async Task<Store> CreateStore(Page page)
        {
            // Wait for element to load.
            var storeNameSelector = @"#store-info-wrap > div.store-container > h3";
            await page.WaitForSelectorAsync(storeNameSelector);

            // Get store name selector.
            var storeNameHandle = await page.QuerySelectorAsync(storeNameSelector);

            // For debug purposes only.
            Debug.WriteLine($"storeNameHandle innerHTML = {await storeNameHandle.GetInnerHtmlAsync()}");

            // Get anchor.
            var anchorHandle = await storeNameHandle.QuerySelectorAsync("a");

            Store store = new Store();
            store.Name = await anchorHandle.GetTextContentAsync();
            store.Link = await anchorHandle.GetAttributeValueAsync("href");

            return store;
        }
    }
}
