﻿using NGAT.Business.Domain.Base;

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
        public ulong FromId { get; set; }

        /// <summary>
        /// The From Node
        /// </summary>
        public Node From { get; set; }

        /// <summary>
        /// The Id of the To Node
        /// </summary>
        public ulong ToId { get; set; }

        /// <summary>
        /// The To Node
        /// </summary>
        public Node To { get; set; }

        /// <summary>
        /// The Data related to this Arc. i.e: Distance, maxspeed, etc
        /// </summary>
        public string ArcData { get; set; }
    }
}