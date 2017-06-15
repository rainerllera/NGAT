using System.Collections.Generic;
using NGAT.Business.Domain.Base;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace NGAT.Business.Domain.Core
{
    /// <summary>
    /// Represents a Network Graph
    /// </summary>
    public class Graph : Entity
    {
        public Graph()
        {
            this.ArcDatas = new List<LinkData>();
            this.NodesIndex = new SortedDictionary<int, Node>();
            this.Nodes = new List<Node>();
            this.Arcs = new List<Arc>();
            this.Edges = new List<Edge>();
        }

        #region Properties
        public string Name { get; set; }
        /// <summary>
        /// The Nodes of this Graph (in Dictionary format, for indexing)
        /// </summary>
        public virtual IDictionary<int, Node> NodesIndex { get; set; }

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
        public virtual IList<LinkData> ArcDatas { get; set; }

        /// <summary>
        /// The Edges of this Graph
        /// </summary>
        public virtual IList<Edge> Edges { get; set; }
        #endregion

        #region Methods
        #region Nodes
        /// <summary>
        /// Adds a node to this graph
        /// </summary>
        /// <param name="node">The node to add</param>
        /// <param name="originalId">The original Id of the node object in its original data source</param>
        /// <param name="fetchedAttributes">The attributes to stores for this node</param>
        public void AddNode(Node node, IDictionary<string, string> fetchedAttributes)
        {

                //Using the well-formed hash code of the coordinate as Id for the node
                node.Id = this.Nodes.Count + 1;//node.Coordinate.GetHashCode(); this is not WORKING

                //Converting the fetched attributes for the node to Json and storing it
                node.NodeData = JsonConvert.SerializeObject(fetchedAttributes);

                //Storing the mapping
                //this.VertexToNodesIndex.Add(originalId, node.Id);

                //Saving the node in collection and index
                NodesIndex.Add(node.Id, node);
                Nodes.Add(node);


          
        }

        /// <summary>
        /// Adds a node to this graph
        /// </summary>
        /// <param name="latitude">The latitude of the node</param>
        /// <param name="longitude">The longitude of the node</param>
        /// <param name="originalId">The original Id of the node object in its original data source</param>
        /// <param name="fetchedAttributes">The attributes to stores for this node</param>
        public void AddNode(double latitude, double longitude, IDictionary<string,string> fetchedAttributes)
        {
            this.AddNode(new Node
            {
                Latitude = latitude,
                Longitude = longitude
            }, fetchedAttributes);
        }
        #endregion

        #region Links
        /// <summary>
        /// Adds an arc to the graph and calculates distance between nodes coordinates
        /// </summary>
        /// <param name="fromOriginalNodeId">The Id of the origin point from data source</param>
        /// <param name="toOriginalNodeId">The Id of the destination point from data source</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        public void AddLink(int fromNodeId, int toNodeId, LinkData linkData, bool directed)
        {
            var fromNode = NodesIndex[fromNodeId];
            var toNode = NodesIndex[toNodeId];
            var distance = fromNode.Coordinate.GetDistanceTo(toNode.Coordinate);
            AddLink(fromNode, toNode, distance, linkData, directed);
        }

        /// <summary>
        /// Adds an arc to the graph with the provided distance (for use when real distance is not the distance between coordinates)
        /// </summary>
        /// <param name="fromOriginalNodeId">The Id of the origin point from data source</param>
        /// <param name="toOriginalNodeId">The Id of the destination point from data source</param>
        /// <param name="distance">Provided distance</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        public void AddLink(int fromNodeId, int toNodeId, double distance, LinkData linkData, bool directed, IEnumerable<Tuple<double, double>> coordinates = null)
        {
            var fromNode = NodesIndex[fromNodeId];
            var toNode = NodesIndex[toNodeId];
            AddLink(fromNode, toNode, distance, linkData, directed,coordinates);
        }

        /// <summary>
        /// Adds an arc to the graph and calculates distance between nodes coordinates
        /// </summary>
        /// <param name="fromNode">The origin node</param>
        /// <param name="toNode">The destination node</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        private void AddLink(Node fromNode, Node toNode, LinkData linkData, bool directed)
        {
            AddLink(fromNode, toNode, fromNode.Coordinate.GetDistanceTo(toNode.Coordinate), linkData, directed);
        }

        /// <summary>
        /// Adds an arc to the graph
        /// </summary>
        /// <param name="fromNode">The origin node</param>
        /// <param name="toNode">The destination node</param>
        /// <param name="distance">Provided distance</param>
        /// <param name="fetchedArcAttributes">The attributes to store for this arc</param>
        private void AddLink(Node fromNode, Node toNode, double distance, LinkData linkData, bool directed, IEnumerable<Tuple<double,double>> coordinates = null)
        {
            if (directed)
            {
                var newArc = new Arc()
                {
                    LinkData = linkData,
                    FromNode = fromNode,
                    ToNode = toNode,
                    FromNodeId = fromNode.Id,
                    ToNodeId = toNode.Id,
                    Graph = this,
                    GraphId = this.Id,
                    Distance = distance,
                    Id = this.Arcs.Count + 1,
                    
                };
                if(coordinates!=null)
                {
                    newArc.PointsData = string.Join(",", coordinates.Select(c => $"{c.Item1} {c.Item2}"));
                }
                fromNode.OutgoingArcs.Add(newArc);
                toNode.IncomingArcs.Add(newArc);
                Arcs.Add(newArc);
            }
            else
            {
                var newEdge = new Edge()
                {
                    LinkData = linkData,
                    FromNode = fromNode,
                    ToNode = toNode,
                    FromNodeId = fromNode.Id,
                    ToNodeId = toNode.Id,
                    Graph = this,
                    GraphId = this.Id,
                    Distance = distance,
                    Id = this.Edges.Count + 1
                };
                if (coordinates != null)
                {
                    newEdge.PointsData = string.Join(",", coordinates.Select(c => $"{c.Item1} {c.Item2}"));
                }
                fromNode.Edges.Add(newEdge);
                toNode.Edges.Add(newEdge);
                Edges.Add(newEdge);
            }
        }

        /// <summary>
        /// Deletes a node from the graph
        /// </summary>
        /// <param name="nodeToDelete">The node to delete</param>
        public void DeleteNode(Node nodeToDelete)
        {
            foreach (var edge in nodeToDelete.Edges)
            {
                if (edge.FromNode == nodeToDelete)
                    edge.ToNode.Edges.Remove(edge);
                else
                    edge.FromNode.Edges.Remove(edge);

                this.Edges.Remove(edge);
            }
            foreach (var arc in nodeToDelete.IncomingArcs)
            {
                arc.FromNode.OutgoingArcs.Remove(arc);
                Arcs.Remove(arc);
            }
            foreach (var arc in nodeToDelete.OutgoingArcs)
            {
                arc.ToNode.IncomingArcs.Remove(arc);
                Arcs.Remove(arc);
            }
            this.Nodes.Remove(nodeToDelete);
            this.NodesIndex.Remove(nodeToDelete.Id);
        }

        /// <summary>
        /// Adds an arcData object to the ArcDataIndex
        /// </summary>
        /// <param name="linkData">The object to index</param>
        public void AddLinkData(LinkData linkData)
        {
            linkData.Id = ArcDatas.Count + 1;
            //ArcDataIndex.Add(arcData.Id, arcData);
            ArcDatas.Add(linkData);
        }
        #endregion
        #endregion

    }
}
