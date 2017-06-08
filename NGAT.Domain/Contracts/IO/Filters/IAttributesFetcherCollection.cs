using System.Collections.Generic;

namespace NGAT.Business.Contracts.Filters
{
    /// <summary>
    /// Represents a collection of AttributesFetchers
    /// </summary>
    public interface IAttributesFetcherCollection : IList<IAttributesFetcher>
    {
        /// <summary>
        /// The union of all AttributesToFetch declared by members of this collection
        /// </summary>
        IEnumerable<string> AllAttributesToFetch { get; }

        /// <summary>
        /// Fetchs all whitelisted attributes from <paramref name="fetchSource"/>
        /// </summary>
        /// <param name="fetchSource">The source to fetch attributes from</param>
        /// <returns>A collection of fetched attributes.</returns>
        IDictionary<string, string> FetchWhiteListed(IDictionary<string, string> fetchSource);
    }
}
