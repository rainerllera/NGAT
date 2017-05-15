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
        /// The Data related to this Arc. i.e: Distance, maxspeed, etc
        /// </summary>
        public string ArcData { get; set; }

        IDictionary<string, string> _arcAttributes;
        /// <summary>
        /// The Deserialized node Data
        /// </summary>
        public IDictionary<string, string> ArcAttributes
        {
            get
            {
                return _arcAttributes ?? (_arcAttributes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(this.NodeData));
            }
        }
    }
}
