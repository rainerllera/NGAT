using System;
using NGAT.Business.Contracts.IO.Filters;

namespace NGAT.Business.Implementation.IO.Osm.Inputs
{
    /// <summary>
    /// Represents the default input for the default OSM-Pbf graph builder
    /// </summary>
    public class OsmPbfGraphBuilderInput
    {
        public OsmPbfGraphBuilderInput(string filePath,
            IAttributeFilterCollection nodeFilters,
            IAttributesFetcherCollection nodeAttributeFetchers,
            IAttributeFilterCollection arcFilters,
            IAttributesFetcherCollection arcAttributeFetchers)
        {
            FilePath = filePath;
            NodeFiltersCollection = nodeFilters ?? throw new ArgumentNullException("nodeFilters");
            NodeAttributeFetchersCollection = nodeAttributeFetchers ?? throw new ArgumentNullException("nodeAttributeFetchers");
            ArcFiltersCollection = arcFilters ?? throw new ArgumentNullException("arcFilters");
            ArcAttributeFetchersCollection = arcAttributeFetchers ?? throw new ArgumentNullException("arcAttributeFetchers");
        }

        /// <summary>
        /// The Path to the .pbf map file
        /// </summary>
        public string FilePath { get; set; }

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
