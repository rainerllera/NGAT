using System.Collections.Generic;
using NGAT.Business.Domain.Base;
using GeoCoordinatePortable;

namespace NGAT.Business.Domain.Core
{
    /// <summary>
    /// Represents a graph node (vertex)
    /// </summary>
    public class Node : GraphDependantEntity
    {
        public Node()
        {
            this.IncomingArcs = new List<Arc>();
            this.OutgoingArcs = new List<Arc>();
            this.Edges = new List<Edge>();
        }

        /// <summary>
        /// The coordinate for this node
        /// </summary>
        public GeoCoordinate Coordinate {
            get
            {
                return new GeoCoordinate(this.Latitude, this.Longitude);
            }
        }

        /// <summary>
        /// The Longitud for this node
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// The Latitude for this node
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The data associated with this node, i.e: Name, Tags, etc, ideally, JSON-encoded
        /// </summary>
        public string NodeData { get; set; }

        /// <summary>
        /// The Deserialized node Data
        /// </summary>
        public IDictionary<string, string> NodeAttributes { get
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(this.NodeData);
            }
        }

        /// <summary>
        /// The outgoing arcs related to this node
        /// </summary>
        public virtual IList<Arc> OutgoingArcs { get; set; }

        /// <summary>
        /// The incoming arcs related to this node
        /// </summary>
        public virtual IList<Arc> IncomingArcs { get; set; }

        /// <summary>
        /// The edges related to this node
        /// </summary>
        public virtual IList<Edge> Edges { get; set; }

        /// <summary>
        /// In-degree
        /// </summary>
        public int InDegree => IncomingArcs.Count;

        /// <summary>
        /// Out-degree
        /// </summary>
        public int OutDegree => OutgoingArcs.Count;

        /// <summary>
        /// Degree
        /// </summary>
        public int Degree => Edges.Count;

    }
}
