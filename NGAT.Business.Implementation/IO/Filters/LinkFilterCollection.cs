using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NGAT.Business.Contracts.IO.Filters;
using System.Linq;

namespace NGAT.Services.IO.Filters
{
    /// <summary>
    /// The default implementation for a road network link filter
    /// </summary>
    public class LinkFilterCollection : List<Func<IDictionary<string, string>, bool>>, IAttributeFilterCollection
    {
       
        public bool ApplyAllFilters(IDictionary<string, string> attributes)
        {
            return this.All(f => f(attributes));
        }
    }
}
