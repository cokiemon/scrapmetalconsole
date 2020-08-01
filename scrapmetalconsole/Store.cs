using System;
using System.Collections.Generic;
using System.Text;

namespace scrapmetalconsole
{
    public class Store
    {
        #region Constructors

        public Store()
        {

        }

        #endregion
        
        /// <summary>
        /// Gets or sets the name of the store.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the link of the store.
        /// </summary>
        public string Link { get; set; }
    }
}
