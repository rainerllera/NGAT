using System;
using System.Threading.Tasks;
using NGAT.Business.Contracts.IO;
using NGAT.Business.Domain.Core;
using NGAT.Business.Implementation.IO.Osm.Inputs;
using System.Collections.Generic;
using System.IO;
using OsmSharp.Streams;
using OsmSharp;
using System.Linq;
using NGAT.Business.Contracts.IO.Filters;

namespace NGAT.Business.Implementation.IO.Osm
{
    
    public class OsmPbfGraphBuilder : IGraphBuilder
    {
        public OsmPbfGraphBuilder(Uri pbfFileURI, 
            IAttributeFilterCollection linkFilters,
            IAttributesFetcherCollection nodeAttrsFetchers,
            IAttributesFetcherCollection linkAttrsFetechers)
        {
            DigitalMapURI = pbfFileURI;
            LinkFilters = linkFilters;
            NodeAttributesFetchers = nodeAttrsFetchers;
            LinkAttributesFetchers = linkAttrsFetechers;
        }

        #region IGraphBuilderMembers
        public Uri DigitalMapURI { get ; set; }

        public IAttributeFilterCollection LinkFilters { get; set; }

        public IAttributesFetcherCollection NodeAttributesFetchers { get; set; }

        public IAttributesFetcherCollection LinkAttributesFetchers { get; set; }

        public Graph Build()
        {
            return InternalBuild();
        }

        public Task<Graph> BuildAsync()
        {
            return new Task<Graph>(() => InternalBuild());
        }
        #endregion

        /// <summary>
        /// Builds the graph from <paramref name="input"/>
        /// </summary>
        /// <param name="input">Input for this GraphBuilder</param>
        /// <returns></returns>
        private Graph InternalBuild()
        {
            if (!File.Exists(DigitalMapURI.LocalPath))
                throw new ArgumentException("Pbf file specified is invalid or doesn't exists.");


            Graph network = new Graph();

            
            var nodeToVertex = new SortedDictionary<long, int>(); //mapping from osm nodes to network's vertexes
            SortedDictionary<long, int> nodesLinkCounter = new SortedDictionary<long, int>(); //the osm nodes with a link counter > 0 will become a network vertex
            List<Way> whiteListedWays = new List<Way>(); //the osm ways that will become links between network's vertexes

            using (var fileStream = File.OpenRead(DigitalMapURI.LocalPath))
            {
                var streamSource = new PBFOsmStreamSource(fileStream);

                #region Whitelisting links and nodes
                foreach (Way way in streamSource.Where(o => o.Type == OsmGeoType.Way))
                {
                    var wayAttrs = way.Tags.ToDictionary(t => t.Key, t => t.Value);
                    if (LinkFilters.ApplyAllFilters(wayAttrs))
                    {
                        whiteListedWays.Add(way);
                        //Whitelisting first Node
                        if (nodesLinkCounter.ContainsKey(way.Nodes[0]))
                            nodesLinkCounter[way.Nodes[0]] += 1;
                        else
                            nodesLinkCounter[way.Nodes[0]] = 1;

                        for (int i = 1; i < way.Nodes.Length - 1; i++)
                        {
                            if (nodesLinkCounter.ContainsKey(way.Nodes[i]))
                                nodesLinkCounter[way.Nodes[i]] += 1;
                            else
                                nodesLinkCounter[way.Nodes[i]] = 0;
                        }

                        //Whitelisting last Node
                        if (nodesLinkCounter.ContainsKey(way.Nodes[way.Nodes.Length - 1]))
                            nodesLinkCounter[way.Nodes[way.Nodes.Length - 1]] += 1;
                        else
                            nodesLinkCounter[way.Nodes[way.Nodes.Length - 1]] = 1;
                    }
                }

                var notAddedNodes = new SortedDictionary<long, OsmSharp.Node>();
                #endregion

                streamSource.Reset();

                #region Adding Relevant Nodes
                foreach (OsmSharp.Node osmNode in streamSource.Where(o => o.Type == OsmGeoType.Node))
                {
                    #region Reading Node Attributes
                    IDictionary<string, string> attributes = new Dictionary<string, string>();
                    foreach (var tag in osmNode.Tags)
                    {
                        attributes.Add(tag.Key, tag.Value);
                    }
                    #endregion

                    #region Filtering node by its link counters
                    if (osmNode.Longitude.HasValue
                        && osmNode.Latitude.HasValue
                        && osmNode.Id.HasValue
                        && nodesLinkCounter.ContainsKey(osmNode.Id.Value)
                        && nodesLinkCounter[osmNode.Id.Value] > 0)
                    {
                        //The node has more than one way that traverses it so we add it to the network
                        var newNode = new Domain.Core.Node()
                        {
                            Latitude = osmNode.Latitude.Value,
                            Longitude = osmNode.Longitude.Value,
                        };

                        //Fetching node attributes
                        var fecthedAttributes = NodeAttributesFetchers.FetchWhiteListed(attributes);

                        //Adding the node to the graph
                        network.AddNode(newNode, fecthedAttributes);

                        nodeToVertex.Add(osmNode.Id.Value, newNode.Id);
                    }
                    else if (osmNode.Longitude.HasValue && osmNode.Latitude.HasValue && osmNode.Id.HasValue)//Conditions for a node to be valid
                    {
                        //We store discarded nodes (with link counter=0) for accuracy in distance calculations
                        notAddedNodes.Add(osmNode.Id.Value, osmNode);
                    }
                    #endregion
                }
                #endregion


                #region Adding Arcs
                foreach (Way osmWay in whiteListedWays)
                {
                    #region Reading way attributes
                    IDictionary<string, string> attributes = new Dictionary<string, string>();
                    foreach (var tag in osmWay.Tags)
                    {
                        attributes.Add(tag.Key, tag.Value);
                    }
                    #endregion

                    #region Filtering ways by its attributes
                    //Way already passed all filters so we process it and fetch the attributes needed
                    var fetchedArcAttributes = LinkAttributesFetchers.FetchWhiteListed(attributes);
                    var arcData = new LinkData()
                    {
                        RawData = Newtonsoft.Json.JsonConvert.SerializeObject(fetchedArcAttributes)
                    };
                    network.AddLinkData(arcData);
                    //Determinig if this way is one-way and if it is, determining it direction
                    bool oneWay = (attributes.ContainsKey("oneway")
                        && attributes["oneway"].ToLowerInvariant() != "no"
                        && attributes["oneway"].ToLowerInvariant() != "0"
                        && attributes["oneway"].ToLowerInvariant() != "false")
                        ||
                        (attributes.ContainsKey("junction")
                        && (attributes["junction"].ToLowerInvariant() == "circular" || attributes["junction"].ToLowerInvariant() == "roundabout"))
                        ||
                        (attributes.ContainsKey("highway") 
                        && ((attributes["highway"].ToLowerInvariant() == "motorway")
                            ||
                            (attributes["highway"].ToLowerInvariant() == "motorway_link") 
                            ||
                            (attributes["highway"].ToLowerInvariant() == "mini_roundabout")
                        ));

                    bool forwardDirection = attributes.ContainsKey("oneway") && (attributes["oneway"].ToLowerInvariant() == "yes"
                        || attributes["oneway"].ToLowerInvariant() == "1"
                        || attributes["oneway"].ToLowerInvariant() == "true");

                    ProcessWay(network, osmWay, oneWay, oneWay ? forwardDirection : true, arcData, nodeToVertex, notAddedNodes);



                    #endregion
                }
                #endregion

            



            }

            return network;
        }

        /// <summary>
        /// Process a way according to its direction
        /// </summary>
        /// <param name="result">The graph being build.</param>
        /// <param name="osmWay">The OSM way to read nodes from</param>
        /// <param name="oneway">A value indicating if this way is one way</param>
        /// <param name="forward">A value indicating if the processing should be made in the same order as the way</param>
        /// <param name="arcData">The fetched arc attributes for this way</param>
        /// <param name="notAddedNodes">The nodes that didn't pass the filters, bu might still be part of a way, necessary for distance calculations.</param>
        private void ProcessWay(Graph result, OsmSharp.Way osmWay, bool oneway, bool forward, LinkData arcData, IDictionary<long, int> nodeToVertexMapping, IDictionary<long, OsmSharp.Node> notAddedNodes)
        {
            var fromNodeId = forward ? osmWay.Nodes[0] : osmWay.Nodes[osmWay.Nodes.Length - 1];
            var initialIterator = forward ? 1 : osmWay.Nodes.Length - 2;
            int iteratorModifier = forward ? 1 : -1;

            for (int i = initialIterator; forward ? i < osmWay.Nodes.Length : i >= 0; i += iteratorModifier)
            {
                var toNodeId = osmWay.Nodes[i];

                if (nodeToVertexMapping.ContainsKey(toNodeId))
                {
                    //Both nodes were added to the graph, so we process the arc
                    result.AddLink(nodeToVertexMapping[fromNodeId], nodeToVertexMapping[toNodeId], arcData, oneway);
                    
                }
                else
                {
                    //The originNode was stored, but not the destination, so we iterate trhough the way untill a stored node is found
                    double accumulatedDistance = 0;
                    var fromNode = result.NodesIndex[nodeToVertexMapping[fromNodeId]];

                    List<Tuple<double, double>> intermediatePoints = new List<Tuple<double, double>>() { new Tuple<double, double>(fromNode.Latitude, fromNode.Longitude) };

                    while (notAddedNodes.TryGetValue(toNodeId, out OsmSharp.Node toNode))
                    {
                        accumulatedDistance += fromNode.Coordinate.GetDistanceTo(new GeoCoordinatePortable.GeoCoordinate(toNode.Latitude.Value, toNode.Longitude.Value));
                        intermediatePoints.Add(new Tuple<double, double>(toNode.Latitude.Value, toNode.Longitude.Value));
                        i += iteratorModifier;
                        toNodeId = osmWay.Nodes[i];
                    }
                    intermediatePoints.Add(new Tuple<double, double>(result.NodesIndex[nodeToVertexMapping[toNodeId]].Latitude, result.NodesIndex[nodeToVertexMapping[toNodeId]].Longitude));
                    result.AddLink(nodeToVertexMapping[fromNodeId], nodeToVertexMapping[toNodeId], accumulatedDistance, arcData, oneway, intermediatePoints);
                }
                fromNodeId = toNodeId;
            }

        }
    }
}
