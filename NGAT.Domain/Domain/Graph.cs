using System;
using System.Collections.Generic;
using System.Text;
using NGAT.Business.Domain.Base;

namespace NGAT.Business.Domain
{
    /// <summary>
    /// Represents a Network Graph
    /// </summary>
    public class Graph : Entity<int>
    {
        /// <summary>
        /// The Nodes of this Graph (in Dictionary format, for indexing)
        /// </summary>
        public virtual IDictionary<ulong, Node> NodesIndex { get; set; }

        /// <summary>
        /// The Arcs of this Graph (in Dictionary format, for indexing)
        /// </summary>
        public virtual IDictionary<ulong, Arc> ArcsIndex { get; set; }

        /// <summary>
        /// The nodes of this graph
        /// </summary>
        public virtual ICollection<Node> Nodes { get; set; }

        /// <summary>
        /// The Arcs of this graph
        /// </summary>
        public virtual ICollection<Arc> Arcs { get; set; }
    }
}
