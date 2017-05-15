using NGAT.Business.Domain.Base;
using System.Collections.Generic;

namespace NGAT.Business.Domain.Core
{
    /// <summary>
    /// A class for representing an arc (directed edge)
    /// </summary>
    public class Arc : GraphDependantEntity
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
        /// The distance this arc covers
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// The id of the data for this arc
        /// </summary>
        public int ArcDataId { get; set; }

        /// <summary>
        /// The data for this arc
        /// </summary>
        public ArcData ArcData { get; set; }

    }
}
