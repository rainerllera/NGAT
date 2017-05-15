using System.Collections.Generic;
using NGAT.Business.Domain.Base;

namespace NGAT.Business.Domain.Core
{
    /// <summary>
    /// Represents a Network Graph
    /// </summary>
    public class Graph : Entity<int>
    {
        public Graph()
        {
            this.VertexToNodesIndex = new SortedDictionary<long, int>();
            this.NodesIndex = new SortedDictionary<int, Node>();
            this.ArcsIndex = new SortedDictionary<int, Arc>();
            this.Nodes = new List<Node>();
            this.Arcs = new List<Arc>();
        }

        #region Properties
        /// <summary>
        /// A mapping to represent the conversion from original points from a map, to graph nodes
        /// </summary>
        public virtual IDictionary<long, int> VertexToNodesIndex { get; set; }

        /// <summary>
        /// The Nodes of this Graph (in Dictionary format, for indexing)
        /// </summary>
        public virtual IDictionary<int, Node> NodesIndex { get; set; }

        /// <summary>
        /// The Arcs of this Graph (in Dictionary format, for indexing)
        /// </summary>
        public virtual IDictionary<int, Arc> ArcsIndex { get; set; }

        /// <summary>
        /// The nodes of this graph
        /// </summary>
        public virtual ICollection<Node> Nodes { get; set; }

        /// <summary>
        /// The Arcs of this graph
        /// </summary>
        public virtual ICollection<Arc> Arcs { get; set; }
        #endregion
    }
}
