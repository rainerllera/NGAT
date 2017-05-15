using System.Collections.Generic;
using NGAT.Business.Domain.Base;
using Newtonsoft.Json;
using System;

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
        /// <summary>
        /// Adds a node to this graph
        /// </summary>
        /// <param name="node">The node to add</param>
        /// <param name="originalId">The original Id of the node object in its original data source</param>
        /// <param name="fetchedAttributes">The attributes to stores for this node</param>
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

        /// <summary>
        /// Adds a node to this graph
        /// </summary>
        /// <param name="latitude">The latitude of the node</param>
        /// <param name="longitude">The longitude of the node</param>
        /// <param name="originalId">The original Id of the node object in its original data source</param>
        /// <param name="fetchedAttributes">The attributes to stores for this node</param>
        public void AddNode(double latitude, double longitude, long originalId, IDictionary<string,string> fetchedAttributes)
        {
            this.AddNode(new Node
            {
                Latitude = latitude,
                Longitude = longitude
            }, originalId, fetchedAttributes);
        }

        /// <summary>
        /// Adds an arc to the graph
        /// </summary>
        /// <param name="fromOriginalNodeId">The Id of the origin point from data source</param>
        /// <param name="toOriginalNodeId">The Id of the destination point from data source</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        public void AddArc(long fromOriginalNodeId, long toOriginalNodeId, IDictionary<string, string> fetchedArcAttributes)
        {
            var fromNode = NodesIndex[VertexToNodesIndex[fromOriginalNodeId]];
            var toNode = NodesIndex[VertexToNodesIndex[toOriginalNodeId]];
            AddArc(fromNode, toNode, fetchedArcAttributes);
        }

        /// <summary>
        /// Adds an arc to the graph
        /// </summary>
        /// <param name="fromNode">The origin node</param>
        /// <param name="toNode">The destination node</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        private void AddArc(Node fromNode, Node toNode, IDictionary<string, string> fetchedArcAttributes)
        {
            var newArc = new Arc()
            {
                ArcData = JsonConvert.SerializeObject(fetchedArcAttributes),
                FromNode = fromNode,
                ToNode = toNode,
                FromNodeId = fromNode.Id,
                ToNodeId = toNode.Id,
                Graph = this,
                GraphId = this.Id,
                Id = this.Arcs.Count + 1
            };
            fromNode.OutgoingArcs.Add(newArc);
            toNode.IncomingArcs.Add(newArc);
            Arcs.Add(newArc);
            ArcsIndex.Add(newArc.Id, newArc);
        }
        #endregion
    }
}
