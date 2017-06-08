using System.Collections.Generic;

namespace NGAT.Business.Contracts.IO.Filters
{
    /// <summary>
    /// Represents an attributes fetcher
    /// </summary>
    public interface IAttributesFetcher
    {
        /// <summary>
        /// A white list of attributes to fetch from an object's attribute (or field) collection
        /// </summary>
        IEnumerable<string> AttributesToFetch { get; set; }
    }
}
