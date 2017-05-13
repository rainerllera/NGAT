using System;
using System.Collections.Generic;
using System.Text;
using NGAT.Business.Contracts.Filters;

namespace NGAT.Business.Implementation.IO.Osm.Inputs
{
    /// <summary>
    /// Represents the default input for the default OSM-Pbf graph builder
    /// </summary>
    public class DefaultOsmPbfGraphBuilderInput
    {
        public DefaultOsmPbfGraphBuilderInput(IAttributeFilterCollection nodeFilters,
            IAttributesFetcherCollection nodeAttributeFetchers,
            IAttributeFilterCollection arcFilters,
            IAttributesFetcherCollection arcAttributeFetchers)
        {
            NodeFiltersCollection = nodeFilters ?? throw new ArgumentNullException("nodeFilters");
            NodeAttributeFetchersCollection = nodeAttributeFetchers ?? throw new ArgumentNullException("nodeAttributeFetchers");
            ArcFiltersCollection = arcFilters ?? throw new ArgumentNullException("arcFilters");
            ArcAttributeFetchersCollection = arcAttributeFetchers ?? throw new ArgumentNullException("arcAttributeFetchers");
        }
        /// <summary>
        /// The Collection of filters to apply to the nodes of the graph
        /// </summary>
        public IAttributeFilterCollection NodeFiltersCollection { get; private set; }

        /// <summary>
        /// The Collection of Fetchers to apply to the nodes of the graph
        /// </summary>
        public IAttributesFetcherCollection NodeAttributeFetchersCollection { get; private set; }

        /// <summary>
        /// The Collection of filters to apply to the arcs of the graph
        /// </summary>
        public IAttributeFilterCollection ArcFiltersCollection { get; private set; }

        /// <summary>
        /// The Collection of Fetchers to apply to the nodes of the graph
        /// </summary>
        public IAttributesFetcherCollection ArcAttributeFetchersCollection { get; private set; }

    }
}
