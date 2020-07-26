using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public abstract class SkuPropertyItemFactory
    {
        public abstract ISkuPropertyItem CreateSkuPropertyItem(string className);

        public abstract Task<ISkuPropertyItem> CreateSkuPropertyItem(ElementHandle elementHandle);
    }
}
