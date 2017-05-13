using System.Collections.Generic;

namespace NGAT.Business.Contracts.Filters
{
    /// <summary>
    /// Abstract representation of a Node Filter by Attributtes.
    /// Filters a node by performing filtering operations to its attributes.
    /// </summary>
    public interface IAttributeFilter
    {
        /// <summary>
        /// Returns true if the object with <paramref name="attributes"/> passes this filter
        /// </summary>
        /// <param name="attributes">The attributes to analyze</param>
        /// <returns></returns>
        bool Filter(IDictionary<string, string> attributes);
    }
}
