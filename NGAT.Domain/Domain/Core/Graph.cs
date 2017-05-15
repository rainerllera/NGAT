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
            this.ArcDataIndex = new SortedDictionary<int, ArcData>();
            this.ArcDatas = new List<ArcData>();
            this.VertexToNodesIndex = new SortedDictionary<long, int>();
            this.NodesIndex = new SortedDictionary<int, Node>();
            this.ArcsIndex = new SortedDictionary<int, Arc>();
            this.Nodes = new List<Node>();
            this.Arcs = new List<Arc>();
        }

        #region Properties
        public virtual IDictionary<int, ArcData> ArcDataIndex { get; set; }
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
        public virtual IList<Node> Nodes { get; set; }

        /// <summary>
        /// The Arcs of this graph
        /// </summary>
        public virtual IList<Arc> Arcs { get; set; }

        /// <summary>
        /// The Arcs profiles for this graph
        /// </summary>
        public virtual IList<ArcData> ArcDatas { get; set; }
        #endregion

        #region Methods
        #region Nodes
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
                node.Id = this.Nodes.Count + 1;//node.Coordinate.GetHashCode(); this is not WORKING

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
        #endregion

        #region Arcs
        /// <summary>
        /// Adds an arc to the graph and calculates distance between nodes coordinates
        /// </summary>
        /// <param name="fromOriginalNodeId">The Id of the origin point from data source</param>
        /// <param name="toOriginalNodeId">The Id of the destination point from data source</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        public void AddArc(long fromOriginalNodeId, long toOriginalNodeId, ArcData arcData)
        {
            var fromNode = NodesIndex[VertexToNodesIndex[fromOriginalNodeId]];
            var toNode = NodesIndex[VertexToNodesIndex[toOriginalNodeId]];
            var distance = fromNode.Coordinate.GetDistanceTo(toNode.Coordinate);
            AddArc(fromNode, toNode, distance, arcData);
        }

        /// <summary>
        /// Adds an arc to the graph with the provided distance (for use when real distance is not the distance between coordinates)
        /// </summary>
        /// <param name="fromOriginalNodeId">The Id of the origin point from data source</param>
        /// <param name="toOriginalNodeId">The Id of the destination point from data source</param>
        /// <param name="distance">Provided distance</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        public void AddArc(long fromOriginalNodeId, long toOriginalNodeId, double distance, ArcData arcData)
        {
            var fromNode = NodesIndex[VertexToNodesIndex[fromOriginalNodeId]];
            var toNode = NodesIndex[VertexToNodesIndex[toOriginalNodeId]];
            AddArc(fromNode, toNode, distance, arcData);
        }

        /// <summary>
        /// Adds an arc to the graph and calculates distance between nodes coordinates
        /// </summary>
        /// <param name="fromNode">The origin node</param>
        /// <param name="toNode">The destination node</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        private void AddArc(Node fromNode, Node toNode, ArcData arcData)
        {
            var newArc = new Arc()
            {
                ArcData = arcData,
                FromNode = fromNode,
                ToNode = toNode,
                FromNodeId = fromNode.Id,
                ToNodeId = toNode.Id,
                Graph = this,
                GraphId = this.Id,
                Distance = fromNode.Coordinate.GetDistanceTo(toNode.Coordinate),
                Id = this.Arcs.Count + 1
            };
            fromNode.OutgoingArcs.Add(newArc);
            toNode.IncomingArcs.Add(newArc);
            Arcs.Add(newArc);
            ArcsIndex.Add(newArc.Id, newArc);
        }

        /// <summary>
        /// Adds an arc to the graph
        /// </summary>
        /// <param name="fromNode">The origin node</param>
        /// <param name="toNode">The destination node</param>
        /// <param name="distance">Provided distance</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        private void AddArc(Node fromNode, Node toNode, double distance, ArcData arcData)
        {
            var newArc = new Arc()
            {
                ArcData = arcData,
                FromNode = fromNode,
                ToNode = toNode,
                FromNodeId = fromNode.Id,
                ToNodeId = toNode.Id,
                Graph = this,
                GraphId = this.Id,
                Distance = distance,
                Id = this.Arcs.Count + 1
            };
            fromNode.OutgoingArcs.Add(newArc);
            toNode.IncomingArcs.Add(newArc);
            Arcs.Add(newArc);
            ArcsIndex.Add(newArc.Id, newArc);
        }

        /// <summary>
        /// Adds an arcData object to the ArcDataIndex
        /// </summary>
        /// <param name="arcData">The object to index</param>
        public void AddArcData(ArcData arcData)
        {
            arcData.Id = ArcDataIndex.Count;
            ArcDataIndex.Add(arcData.Id, arcData);
            ArcDatas.Add(arcData);
        }
        #endregion
        #endregion

    }
}
