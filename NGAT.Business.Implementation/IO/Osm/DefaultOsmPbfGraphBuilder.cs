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

namespace NGAT.Business.Implementation.IO.Osm
{
    public class DefaultOsmPbfGraphBuilder : IGraphBuilder<DefaultOsmPbfGraphBuilderInput>
    {
        public Graph Build(DefaultOsmPbfGraphBuilderInput input)
        {
            return InternalBuild(input);
        }

        public Task<Graph> BuildAsync(DefaultOsmPbfGraphBuilderInput input)
        {
            return new Task<Graph>(() => InternalBuild(input));
        }

        private Graph InternalBuild(DefaultOsmPbfGraphBuilderInput input)
        {
            if (!File.Exists(input.FilePath))
                throw new ArgumentException("Pbf file specified is invalid or doesn't exists.");

            Graph result = new Graph();

            using (var fileStream = File.OpenRead(input.FilePath))
            {
                var streamSource = new PBFOsmStreamSource(fileStream);
                var notAddedNodes = new SortedDictionary<long, OsmSharp.Node>();
                #region Adding Nodes (filtered)
                foreach (OsmSharp.Node osmNode in streamSource.Where(o=>o.Type == OsmGeoType.Node))
                {
                    #region Reading Node Attributes
                    IDictionary<string, string> attributes = new Dictionary<string, string>();
                    foreach (var tag in osmNode.Tags)
                    {
                        attributes.Add(tag.Key, tag.Value);
                    }
                    #endregion

                    #region Filtering node by its attributes
                    if (input.NodeFiltersCollection.ApplyAllFilters(attributes) && osmNode.Longitude.HasValue && osmNode.Latitude.HasValue && osmNode.Id.HasValue)
                    {
                        var newNode = new Domain.Core.Node()
                        {
                            Latitude = osmNode.Latitude.Value,
                            Longitude = osmNode.Longitude.Value,

                        };

                        //Fetching node attributes
                        var fecthedAttributes = input.NodeAttributeFetchersCollection.FetchWhiteListed(attributes);

                        //Adding the node to the graph
                        result.AddNode(newNode, osmNode.Id.Value, fecthedAttributes);
                    }
                    else if(osmNode.Longitude.HasValue && osmNode.Latitude.HasValue && osmNode.Id.HasValue)//Conditions for a node to be valid
                    {
                        //We store discarded nodes for accuracy in distance calculations
                        notAddedNodes.Add(osmNode.Id.Value, osmNode);
                    }
                    #endregion
                }
                #endregion
                streamSource.Reset();
                #region Adding Arcs
                foreach (Way osmWay in streamSource.Where(o=>o.Type==OsmGeoType.Way))
                {
                    #region Reading way attributes
                    IDictionary<string, string> attributes = new Dictionary<string, string>();
                    foreach (var tag in osmWay.Tags)
                    {
                        attributes.Add(tag.Key, tag.Value);
                    }
                    #endregion

                    #region Filtering ways by its attributes
                    if(input.ArcFiltersCollection.ApplyAllFilters(attributes))
                    {
                        //Way passes all filters so we process it and fetch the attributes needed
                        var fetchedArcAttributes = input.ArcAttributeFetchersCollection.FetchWhiteListed(attributes);

                        //Determinig if this way is one-way and if it is, determining it direction
                        bool oneWay = attributes.ContainsKey("oneway")
                            && attributes["oneway"].ToLowerInvariant() != "no"
                            && attributes["oneway"].ToLowerInvariant() != "0"
                            && attributes["oneway"].ToLowerInvariant() != "false";
                            
                        bool forwardDirection = oneWay && (attributes["oneway"].ToLowerInvariant() == "yes" 
                            || attributes["oneway"].ToLowerInvariant() == "1" 
                            || attributes["oneway"].ToLowerInvariant() == "true");

                        #region Adding Arcs
                        if(oneWay)
                        {
                            #region One Way
                            //Way is one way, adding arcs according to direction
                            var fromNodeId = forwardDirection ? osmWay.Nodes[0] : osmWay.Nodes[osmWay.Nodes.Length - 1];
                            var initialIterator = forwardDirection ? 1 : osmWay.Nodes.Length - 2;
                            int iteratorModifier = forwardDirection ? 1 : -1;

                            for (int i = initialIterator; forwardDirection ? i < osmWay.Nodes.Length : i >= 0; i+=iteratorModifier)
                            {
                                var toNodeId = osmWay.Nodes[i];

                                if (result.VertexToNodesIndex.ContainsKey(fromNodeId) && result.VertexToNodesIndex.ContainsKey(toNodeId))
                                {
                                    //Both nodes were added to the graph, so we process the arc
                                    result.AddArc(fromNodeId, toNodeId, fetchedArcAttributes);
                                    fromNodeId = toNodeId;
                                }
                                else if(result.VertexToNodesIndex.ContainsKey(fromNodeId))
                                {
                                    //The originNode was stored, but not the destination, so we iterate trhough the way untill a stored node is found
                                    double accumulatedDistance = 0;
                                    var fromNode = result.NodesIndex[result.VertexToNodesIndex[fromNodeId]];

                                    OsmSharp.Node toNode;
                                    bool foundFlag = true;
                                    while (notAddedNodes.TryGetValue(toNodeId, out toNode))
                                    {
                                        accumulatedDistance += fromNode.Coordinate.GetDistanceTo(new GeoCoordinatePortable.GeoCoordinate(toNode.Latitude.Value, toNode.Longitude.Value));
                                        i += iteratorModifier;
                                        if (forwardDirection ? i < osmWay.Nodes.Length : i >= 0)
                                        {
                                            foundFlag = false;
                                            break;
                                        }
                                        toNodeId = osmWay.Nodes[i];
                                    }
                                    if (foundFlag)
                                    {
                                        result.AddArc(fromNodeId, toNodeId, accumulatedDistance, fetchedArcAttributes);
                                        fromNodeId = toNodeId;
                                    }
                                    else
                                    {
                                        //This way is worthless, we skip it completly
                                        break;
                                    }
                                    
                                }
                                else
                                {
                                    //Neither the origin node nor the destination node were stored, so we'll store whatever we can about this way
                                    int fromInternalNodeId;
                                    while (!result.VertexToNodesIndex.TryGetValue(fromNodeId, out fromInternalNodeId))
                                    {
                                        //We skip
                                        i += iteratorModifier;
                                        fromNodeId = osmWay.Nodes[i];
                                    }
                                }

                                
                            }
                            #endregion
                        }
                        else
                        {
                            #region Both ways
                            //Way is not one way, adding arcs in both directions

                            #endregion


                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }

            return result;
        }
    }
}
