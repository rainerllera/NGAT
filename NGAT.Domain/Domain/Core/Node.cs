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
        private GeoCoordinate _coordinate;
        /// <summary>
        /// The coordinate for this node
        /// </summary>
        public GeoCoordinate Coordinate {
            get
            {
                return _coordinate ?? (_coordinate = new GeoCoordinate(this.Latitude, this.Longitude));
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
        /// The outgoing arcs related to this node
        /// </summary>
        public virtual ICollection<Arc> OutgoingArcs { get; set; }

        /// <summary>
        /// The incoming arcs related to this node
        /// </summary>
        public virtual ICollection<Arc> IncomingArcs { get; set; }
    }
}
