using System;
using System.Collections.Generic;
using System.Text;
using NGAT.Business.Domain.Core;

namespace NGAT.Business.Contracts.Common
{
    /// <summary>
    /// Represents a Type that encloses a Graph object
    /// </summary>
    public interface IGraphContainer
    {
        /// <summary>
        /// The graph this object wraps
        /// </summary>
        Graph Graph { get; set; }
    }
}
