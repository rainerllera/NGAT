﻿using System;
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

        /// <summary>
        /// Builds the graph from <paramref name="input"/>
        /// </summary>
        /// <param name="input">Input for this GraphBuilder</param>
        /// <returns></returns>
        private Graph InternalBuild(DefaultOsmPbfGraphBuilderInput input)
        {
            if (!File.Exists(input.FilePath))
                throw new ArgumentException("Pbf file specified is invalid or doesn't exists.");

            
            Graph result = new Graph();

            //mapping from nodes to vertex
            var nodeToVertex = new SortedDictionary<long, int>();

            using (var fileStream = File.OpenRead(input.FilePath))
            {
                var streamSource = new PBFOsmStreamSource(fileStream);
                SortedSet<long> whiteListedNodes = new SortedSet<long>();
                List<Way> whiteListedWays = new List<Way>();
                foreach (Way way in streamSource.Where(o=>o.Type==OsmGeoType.Way))
                {
                    var wayAttrs = way.Tags.ToDictionary(t => t.Key, t => t.Value);
                    if(input.ArcFiltersCollection.ApplyAllFilters(wayAttrs))
                    {
                        whiteListedWays.Add(way);
                        foreach (var nodeId in way.Nodes)
                        {
                            whiteListedNodes.Add(nodeId);
                        }
                    }
                }

                var notAddedNodes = new SortedDictionary<long, OsmSharp.Node>();

                streamSource.Reset();

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
                    if (osmNode.Longitude.HasValue && osmNode.Latitude.HasValue && osmNode.Id.HasValue && whiteListedNodes.Contains(osmNode.Id.Value) && input.NodeFiltersCollection.ApplyAllFilters(attributes))
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

                        nodeToVertex.Add(osmNode.Id.Value, newNode.Id);
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
                    if (input.ArcFiltersCollection.ApplyAllFilters(attributes))
                    {
                        //Way passes all filters so we process it and fetch the attributes needed
                        var fetchedArcAttributes = input.ArcAttributeFetchersCollection.FetchWhiteListed(attributes);
                        var arcData = new ArcData()
                        {
                            RawData = Newtonsoft.Json.JsonConvert.SerializeObject(fetchedArcAttributes)
                        };
                        result.AddArcData(arcData);
                        //Determinig if this way is one-way and if it is, determining it direction
                        bool oneWay = attributes.ContainsKey("oneway")
                            && attributes["oneway"].ToLowerInvariant() != "no"
                            && attributes["oneway"].ToLowerInvariant() != "0"
                            && attributes["oneway"].ToLowerInvariant() != "false";

                        bool forwardDirection = oneWay && (attributes["oneway"].ToLowerInvariant() == "yes"
                            || attributes["oneway"].ToLowerInvariant() == "1"
                            || attributes["oneway"].ToLowerInvariant() == "true");

                        #region Adding Arcs
                        if (oneWay)
                        {
                            #region One Way
                            //Way is one way, adding arcs in corresponding direction
                            ProcessWay(result, osmWay, forwardDirection, arcData, nodeToVertex, notAddedNodes);
                            #endregion
                        }
                        else
                        {
                            #region Both ways
                            //Way is not one way, adding arcs in both directions
                            ProcessWay(result, osmWay, true, arcData, nodeToVertex, notAddedNodes);
                            ProcessWay(result, osmWay, false, arcData, nodeToVertex, notAddedNodes);
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

        /// <summary>
        /// Process a way according to its direction
        /// </summary>
        /// <param name="result">The graph being build.</param>
        /// <param name="osmWay">The OSM way to read nodes from</param>
        /// <param name="forward">A value indicating if the processing should be made in the same order as the way</param>
        /// <param name="arcData">The fetched arc attributes for this way</param>
        /// <param name="notAddedNodes">The nodes that didn't pass the filters, bu might still be part of a way, necessary for distance calculations.</param>
        private void ProcessWay(Graph result, OsmSharp.Way osmWay, bool forward, ArcData arcData, IDictionary<long, int> nodeToVertexMapping, IDictionary<long,OsmSharp.Node> notAddedNodes)
        {
            var fromNodeId = forward ? osmWay.Nodes[0] : osmWay.Nodes[osmWay.Nodes.Length - 1];
            var initialIterator = forward ? 1 : osmWay.Nodes.Length - 2;
            int iteratorModifier = forward ? 1 : -1;

            for (int i = initialIterator; forward ? i < osmWay.Nodes.Length : i >= 0; i += iteratorModifier)
            {
                var toNodeId = osmWay.Nodes[i];

                if (nodeToVertexMapping.ContainsKey(fromNodeId) && nodeToVertexMapping.ContainsKey(toNodeId))
                {
                    //Both nodes were added to the graph, so we process the arc
                    result.AddArc(nodeToVertexMapping[fromNodeId], nodeToVertexMapping[toNodeId], arcData);
                    fromNodeId = toNodeId;
                }
                else if (nodeToVertexMapping.ContainsKey(fromNodeId))
                {
                    //The originNode was stored, but not the destination, so we iterate trhough the way untill a stored node is found
                    double accumulatedDistance = 0;
                    var fromNode = result.NodesIndex[nodeToVertexMapping[fromNodeId]];

                    bool foundFlag = true;
                    while (notAddedNodes.TryGetValue(toNodeId, out OsmSharp.Node toNode))
                    {
                        accumulatedDistance += fromNode.Coordinate.GetDistanceTo(new GeoCoordinatePortable.GeoCoordinate(toNode.Latitude.Value, toNode.Longitude.Value));
                        i += iteratorModifier;
                        if (forward ? i < osmWay.Nodes.Length : i >= 0)
                        {
                            foundFlag = false;
                            break;
                        }
                        toNodeId = osmWay.Nodes[i];
                    }
                    //Accumulated distance>0 means that at least one node was found in the NotAddedNodes index
                    //Verifying that we stored toNode, cause can happen that previous loop exited because the current node wasn't in notAddedNodes,
                    //and that does not imply that we stored toNodeId
                    if (foundFlag && accumulatedDistance > 0 && nodeToVertexMapping.ContainsKey(toNodeId))
                    {
                        result.AddArc(nodeToVertexMapping[fromNodeId], nodeToVertexMapping[toNodeId], arcData);
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
                    //That is, we'll look up the first stored node in the way, if any, and try to process the rest of the way
                   
                    while (!nodeToVertexMapping.ContainsKey(fromNodeId))
                    {
                        //We skip
                        i += iteratorModifier;
                        if (i >= osmWay.Nodes.Length || i < 0)
                            break;
                        fromNodeId = osmWay.Nodes[i];
                    }
                }


            }
        }
    }
}
