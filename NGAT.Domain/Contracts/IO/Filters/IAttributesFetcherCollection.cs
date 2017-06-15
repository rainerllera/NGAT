using System.Collections.Generic;
using System;

namespace NGAT.Business.Contracts.IO.Filters
{
    /// <summary>
    /// Represents a collection of AttributesFetchers
    /// </summary>
    public interface IAttributesFetcherCollection : IList<Func<IDictionary<string,string>,IEnumerable<KeyValuePair<string,string>>>>
    {
        /// <summary>
        /// Fetchs all whitelisted attributes from <paramref name="fetchSource"/>
        /// </summary>
        /// <param name="fetchSource">The source to fetch attributes from</param>
        /// <returns>A collection of fetched attributes.</returns>
        IDictionary<string, string> Fetch(IDictionary<string, string> fetchSource);
    }
}
