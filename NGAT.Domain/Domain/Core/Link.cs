using NGAT.Business.Domain.Base;
using System.Collections.Generic;
using System;

namespace NGAT.Business.Domain.Core
{
    public abstract class Link : GraphDependantEntity
    {
        /// <summary>
        /// The Id of the From Node
        /// </summary>
        public int FromNodeId { get; set; }

        /// <summary>
        /// The From Node
        /// </summary>
        public Node FromNode { get; set; }

        /// <summary>
        /// The Id of the To Node
        /// </summary>
        public int ToNodeId { get; set; }

        /// <summary>
        /// The To Node
        /// </summary>
        public Node ToNode { get; set; }

        /// <summary>
        /// The distance this link covers
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// The id of the data for this link
        /// </summary>
        public int LinkDataId { get; set; }

        /// <summary>
        /// The data for this arc
        /// </summary>
        public LinkData LinkData { get; set; }

        /// <summary>
        /// The sequence of points that compose this link
        /// </summary>
        public string PointsData { get; set; }

        /// <summary>
        /// A value indicating wether this link is directed (an arc) or not (an edge)
        /// </summary>
        public abstract bool Directed { get; }
    }

    /// <summary>
    /// Represents an arc
    /// </summary>
    public class Arc : Link
    {
        public override bool Directed => true;
        public override string ToString()
        {
            return $"[{FromNodeId}]-->[{ToNodeId}]";
        }
    }

    /// <summary>
    /// Represents an edge
    /// </summary>
    public class Edge : Link
    {
        public override bool Directed => false;
        public override string ToString()
        {
            return $"{{{FromNodeId}]---[{ToNodeId}}}";
        }
    }
}
