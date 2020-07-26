using System;
using System.Collections.Generic;
using System.Text;

namespace scrapmetalconsole
{
    public interface ISkuPropertyItem
    {
        string ClassName { get; set; }

        bool IsSelected { get; set; }
    }
}
