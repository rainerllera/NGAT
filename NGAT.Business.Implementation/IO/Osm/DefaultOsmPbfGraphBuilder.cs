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
                    #endregion
                }
                #endregion

                #region Adding Arcs

                #endregion
            }

            return result;
        }
    }
}
