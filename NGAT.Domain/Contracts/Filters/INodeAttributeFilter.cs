using System;
using System.Collections.Generic;
using System.Text;

namespace NGAT.Business.Contracts.Filters
{
    /// <summary>
    /// Abstract representation of a Node Filter by Attributtes.
    /// Filters a node by performing filtering operations to its attributes.
    /// </summary>
    public interface INodeAttributeFilter
    {
        bool Filter(IDictionary<string, string> attributes);
    }
}
