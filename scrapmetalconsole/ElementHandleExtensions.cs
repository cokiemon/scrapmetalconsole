using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PuppeteerSharp;

namespace scrapmetalconsole
{
    public static class ElementHandleExtensions
    {
        public static async Task<string> GetInnerHtmlAsync(this ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException("elementHandle");
            }

            return await elementHandle.EvaluateFunctionAsync<string>("e => e.innerHTML");
        }

        public static async Task<string> GetTextContentAsync(this ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException("elementHandle");
            }

            return await elementHandle.EvaluateFunctionAsync<string>("e => e.textContent");
        }

        public static async Task<string> GetClassNameAsync(this ElementHandle elementHandle)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException("elementHandle");
            }

            return await elementHandle.EvaluateFunctionAsync<string>("e => e.className");
        }

        public static async Task<string> GetAttributeValueAsync(this ElementHandle elementHandle, string attribute)
        {
            if (elementHandle == null)
            {
                throw new ArgumentNullException("elementHandle");
            }

            if (string.IsNullOrEmpty(attribute))
            {
                throw new ArgumentException("Attribute must be non-empty.", "attribute");
            }

            string script = "e => e.getAttribute('')";

            return await elementHandle.EvaluateFunctionAsync<string>(script.Insert(21, attribute));
        }
    }
}
