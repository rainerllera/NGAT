using System.Collections.Generic;
using NGAT.Business.Domain.Base;
using Newtonsoft.Json;

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

        #region Methods
        public void AddNode(Node node, long originalId, IDictionary<string, string> fetchedAttributes)
        {
            if(!this.NodesIndex.ContainsKey(node.Coordinate.GetHashCode()))
            {
                //Using the well-formed hash code of the coordinate as Id for the node
                node.Id = node.Coordinate.GetHashCode();

                //Converting the fetched attributes for the node to Json and storing it
                node.NodeData = JsonConvert.SerializeObject(fetchedAttributes);

                //Storing the mapping
                this.VertexToNodesIndex.Add(originalId, node.Id);

                //Saving the node in collection and index
                NodesIndex.Add(node.Id, node);
                Nodes.Add(node);


            }
        }
        #endregion
    }
}
